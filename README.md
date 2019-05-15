Some information:
•	A dedicated client id/secret  and Opisense user were created for connection to the API;
•	Those are defined in the attached project in “Program.cs”;
•	The actual variables management is done in “StandardVariableProcessor.cs”;
•	Some sample code is there to help programmers get started with the logic implementation for the variables creation;
•	The code was tested on the single source you currently created on the account (you can check on the output and delete those created variables as you wish);
•	You can test the code locally by simply executing it while changing “.WithWhatifBypass(false)” to “.WithWhatifBypass(true)” in Program.cs and specifying the source that you want to process “.WithDefaultAdditionalProperties(new { Id = 227077 })” where 227077 is, as you can check, the id of the currently existing source;
•	Make sure to reset the WithWhatifBypass to false before compiling the code and loading it to Opisense (obviously, when run from Opisense, the sourceId will be passed on to the code in the Master Data process);
•	To use the customizer in the Master Data process of Opisense, you must:
o	Compile the code and generate the “Standard.MasterDataCustomization.exe” file
o	That file must be specified in the Master Data template as “CustomizationAssembly” in the config tab (or equivalent field in JSON format)
o	The .exe file must be loaded to the Opisense storage BUT pay attention to renaming the file to an allowed extension, typically “Standard.MasterDataCustomization.zip”, to load it from the interface (DON’T ZIP the file, just rename the extension)

When running master data, as defined in the template, the variable customizer will be called to create the variables according to the implemented logic (you should probably pay attention not to recreate or even update the variables upon source updates – we usually define a specific “ForceUpdate” form value to trigger variable updates on sources when required).

Also note that the customizer will only be called for an existing source when an update is detected in the source definition. If a source listed in the Master Data file exists and presents no change when compared to the Opisense existing source, the customizer will not be called.
