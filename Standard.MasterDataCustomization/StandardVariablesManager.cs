using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Standard.MasterDataCustomization.Model;
using MasterDataCustomization;
using Newtonsoft.Json.Linq;

namespace Standard.MasterDataCustomization
{
    public partial class StandardVariablesManager
    {
        //Set the account Id used to test in manual mode
        private const int AccountId = 999;
#if __DEBUG
        private const bool outputJsonInsteadOfCreatingVariables = true;
        private const string outputJsonFileName = "StandardVariables.json";
#else
        private const bool outputJsonInsteadOfCreatingVariables = false;
#endif
        public static async Task UpdateStandardVariablesConfiguration(AuthenticatedClient client, CustomizationContext ctx, Action<string> onFeedback)
        {
            try
            {
                if (ctx.Step == CustomizationSteps.Manual)
                {
                    onFeedback = async msg => { await Console.Out.WriteLineAsync(msg); };
                }
                else if (onFeedback == null)
                    onFeedback = _ => { };

                var helper = new CustomizationHelper(client, ctx, onFeedback);

                switch (ctx.Step)
                {
                    case CustomizationSteps.Manual:
                        if (helper.WhatIf)
                        {
                            helper.SendFeedback("***********************************************************");
                            helper.SendFeedback("**  RUNNING IN WHATIF MODE, NO CHANGES WILL BE APPLIED   **");
                            helper.SendFeedback("***********************************************************");
                        }
                        if (outputJsonInsteadOfCreatingVariables)
                        {
                            helper.SendFeedback("***********************************************************");
                            helper.SendFeedback("**  PRODUCTION OF A JSON FILE, NO VARIABLES CREATION     **");
                            helper.SendFeedback("***********************************************************");
                        }
                        if (SingleSourceIdIsSpecified(helper))
                            await HandleManualCustomizationOnSingleSourceId(helper);
                        else
                            await HandleManualCustomizationOnAllSources(helper);
                        await Console.Out.WriteLineAsync("Press any key to exit...");
                        Console.ReadKey();
                        break;
                    case CustomizationSteps.AfterSourceCreation:
                    case CustomizationSteps.AfterSourceMove:
                    case CustomizationSteps.AfterSourceUpdate:
                        await HandleAutomatedCustomization(helper);
                        break;
                    default:
                        //helper.SendFeedback($"The step {ctx.Step} is not implemented in Eoly customization");
                        return;
                }
            }
            catch (Exception ex)
            {
                onFeedback?.Invoke($"Exception handling customization event <{ctx.Step}> for business key<{ctx.BusinessKey}> on account <{ctx.AccountId}>: {ex}");
            }
        }

        private static bool SingleSourceIdIsSpecified(CustomizationHelper helper)
        {
            return !string.IsNullOrEmpty(helper.StringValueOf(helper.Context.AdditionalProperties, "Id"));
        }

        private static async Task HandleManualCustomizationOnSingleSourceId(CustomizationHelper helper)
        {
            helper.SendFeedback("Handling single source");
            await HandleAutomatedCustomization(helper);
        }

        private static async Task HandleManualCustomizationOnAllSources(CustomizationHelper helper)
        {
            helper.SendFeedback("Handling all sources having no variable");
            const int pageSize = 500;
            var account = await helper.GetAccount(AccountId);
            var variablesList = new ConcurrentBag<Variable>();

            var processedCount = 0;
            var tasks = new List<Task>();
            foreach (var sourceIdsToProcess in helper.Split(account.Sources, pageSize))
            {
                tasks.Add(Task.Run(async () =>
                {
                    Interlocked.Add(ref processedCount, sourceIdsToProcess.Count);
                    helper.SendFeedback($"Start processing {sourceIdsToProcess.Count} sources, {processedCount} sources processed so far");

                    var sourceIdsWithoutVariables = (await helper.GetSourceIdsWithoutVariables(sourceIdsToProcess)).ToList();
                    if (sourceIdsWithoutVariables.Any())
                    {
                        var sourcesToProcess = (await helper.GetSources(sourceIdsWithoutVariables.Distinct())).ToList();
                        if (sourcesToProcess.Any())
                        {
                            if (helper.WhatIf)
                            {
                                helper.SendFeedback($"Processing {sourcesToProcess.Count} sources having no variable");
                            }

                            foreach (var source in sourcesToProcess)
                            {
                                var vars = await Process(source, new List<string>(), helper, outputJsonInsteadOfCreatingVariables);
                                foreach (var v in vars)
                                    variablesList.Add(v);
                            }
                        }
                    }
                }));
            };

            await Task.WhenAll(tasks);
#if __DEBUG
            helper.SendFeedback($"Serializing {variablesList.Count} variables to file '{new FileInfo(outputJsonFileName).FullName}'");
            File.WriteAllText(outputJsonFileName, JsonConvert.SerializeObject(variablesList));
            helper.SendFeedback($"End of process (lasted {(DateTime.UtcNow - start):g})");
#endif

        }

        private static async Task HandleAutomatedCustomization(CustomizationHelper helper)
        {
            var sourceIdStr = helper.EnsureValid(helper.StringValueOf(helper.Context.AdditionalProperties, "Id"), s => s != null,
                $"The source Id is missing in the customization context: {helper.Context}");
            var sourceVariables = helper.ArrayValueOf(helper.Context.AdditionalProperties, "Variables");

            //Get source Id
            helper.EnsureValid(int.TryParse(sourceIdStr, out var sourceId), v => v,
                $"The source Id<{sourceIdStr}> coming from the customization context is not convertible to an interger");

            var source = (await helper.GetSources(new[] { sourceId })).FirstOrDefault();
            if (source is null)
            {
                helper.SendFeedback($"The source with Id<{sourceId}> does not exist in the database");
                if (!helper.WhatIf)
                    return;

                //In whatif mode, create a fake source
                source = new Source
                {
                    Id = 0,
                    EanNumber = "FAKE EAN",
                    ClientData = JObject.Parse("{'No client data'}"),
                    Name = "FAKE NAME",
                    SiteId = 0,
                    SiteName = "NO SITE"
                };
            }
            await Process(source, sourceVariables, helper, false);
        }
    }
}