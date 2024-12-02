# Welcome-To-Tarkov: Client Section

Anything related to the client belongs in this directory

# How to add a new module to the solution

1) Copy the `!Project Template - DO NOT EDIT THIS` folder and paste it in the client folder, Where `Welcome To Takov.sln` lives.
2) Rename the folder to the Module name.
3) Go into the newly renamed folder and rename the Template.csproj file to the same name as the parent folder.
4) Inside of the solution in visual studio(2022) right click on `Solution 'Welcome To Tarkov'` in the solution explorer and select add->Existing project.
 


5) Navigate to where you created the new module from the template and select the *.csproj file you changed the name of select it and click open.



6) Congrats you've now added a new module to the solution all set up with the correct dependencies for tarkov.

6a) I use post build events to copy the dll from the bin folder to the pugins folder everytime I build so I dont need to manually copy.
Everyones install is in a different location and I CANNOT automate this or I would. You will need to edit the post build event according to your own pathing.
Open the *.csproj file of all modules and edit this section:

        <Exec Command="copy &quot;$(TargetPath)&quot; &quot;E:\SPT_371\BepInEx\plugins\WelcomeToTarkov\$(TargetName).dll&quot;&#xD;&#xA;if $(ConfigurationName) == Debug (&#xD;&#xA;    copy &quot;$(ProjectDir)$(OutDir)$(TargetName).pdb&quot; &quot;E:\SPT_371\BepInEx\plugins\WelcomeToTarkov\$(TargetName).pdb&quot;&#xD;&#xA;) else (&#xD;&#xA;    del &quot;E:\SPT_371\BepInEx\plugins\WelcomeToTarkov$(TargetName).pdb&quot;&#xD;&#xA;)&#xD;&#xA;&#xD;&#xA;del &quot;G:\3.6.1 -                                                 
        Dev\BepInEx\config\com.dirtbikercj.$(TargetName).cfg&quot;" />

6b) Everywhere you see `E:\SPT_371` replace that with the path to the root of your own install. Once done Visual studio will copy the built dlls to the plugins\WelcomeToTarkov folder.

# Building the project

1) Open the solution in Visual studio 2022.
2) In the solution explorer right click on the solution and build solution, or press F7.

3) If you've done step 6a and 6b the plugin will be automatically copied to the plugins folder.
