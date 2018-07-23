This project was create to prove my skills to Siemens company.

It can create and process huge XML file, with Persons with Name, Surename, Address and Birthday.
It can load the XML file, or generate the file with prefered size randomly from meaningful stored data, or you can create the data manually.
Then the data are shown and could be edited, deleted (could be more rows) or added.
Then the data could be saved, or if it's needed, reset to last saved state.
Those things could be done by key binding. And you will be asked, if you would like to save data before close the app.
The grid can order the data or/and group them.

On second tab, there are some statistics:
- 10 most common names in the list, with it's count
- 10 most common surenames in the list, with it's count
- Names and surenames of people, which celebrate this day a birthday
Statistics are counted on load, and then on data save. It's not counted from online data due to optimalization, but from data from "data warehouse".

The project uses MVVM pattern, and Ioc container. 
All the commands have can execute conditions, if they are needed, and the are in most way asynchronous, so they don't block the GUI.
Progress of generating data and loadin them is reported, saving doesn't reports properly.

The GUI is fully styled and responsive, all the resources are centered and not duplicated as much as they can. 
There are no hard coded strings, all of them are in Resources.resx, so the program can be easily localizated.

There are some Unit tests, in separate project too.
Unfortunately there are no functional GUI tests, it was tested manually. 

In the future it will be nice to add GUI tests, and implement search function.