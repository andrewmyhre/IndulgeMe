Bless The Web
=============

Solution Folders
/BlessTheWeb - Main website
/BlessTheWeb.Core - Repository and domain classes
/BlessTheWeb.Tests - a few tests on primary logic
/BlessTheWeb.Trawler - application for mining internet 'sin' databases. Currently only 1 trawler supported (TextFromLastNight) and some refactoring required to decouple this
/BlessTheWeb.Trawler.Tests - tests for trawler logic
/BlessTheWeb.Twitter - responsible for twitter communication
/RavenDb.Server - server executable for the database
/libs - common assemblies

To run the website:

Build the solution.
Locate /RavenDb.Server/RavenDb.Server.exe and start it
Once RavenDb server is running, locate /BlessTheWeb.Trawler/bin/debug/BlessTheWeb.Trawler.Exe and execute it.
Once the trawler has finished ("Press any key to continue...") run the website.

Any questions hit me on twitter: @andrewmy

Credits
=======
@andrewmy 		Andrew Myhre 	- initial spike
@kaichanvong 	Kai Chan Vong 	- design
@davidwhitney	David Whitney	- creative direction
@robcooper		Rob Cooper		- creative direction

