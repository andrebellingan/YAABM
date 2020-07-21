# Yet Another Agent-Based Model (YAABM)

This is an agent based model that is being developed to model the impact of the COVID-19 pandemic in South Africa.

---

## Getting started

### System requirements
* Windows 10 (must be 64-bit)
* .NET Framework 4.7.2 runtimes
* Sufficient RAM given the size of the simulation and the number of processing threads (see below)

### Compiling to code

1. Download or clone the source code
2. Compiling
	* It is recommended to open the solution in Visual Studio 2017 (or 2019) and build the projected from there (The Visual Studio Community editions are free to download for non-commercial use)
	* Alternatively you can download the .NET SDK and use ```dotnet build CAMB.sln``` from the command line (remember to get the correct version of the SDK)
3. Note that the nuget package manager may indicate that there is an update available for the "CommandLineArgumentsParser" package. Due to a bug in version 3.0.20 of the package when handling default argument values you should not upgrade to this specific version.

### Running the model

To run the model you need to execute **RunCovidSimulation.exe** from the command line.

The application has the following command line arguments:

	-n, --iterations  [Required] How many Monte Carlo iterations to simulate

	-s, --scenario    [Required] The path to the scenario settings file

	-t, --threads     [Optional] the number of parallel threads to use. 

	-d, --saveDate    [Optional] Switch to add the date and time to output file names


example: `RunCovidSimulation.exe -n 100 -s ./Scenarios/Baseline_scenario.json -t 4`

### Threading
The default value for the number of threads is `-t 1`. Running single threaded is highly recommended for debugging. Setting `-t 0` will detect the number of logical cores on the system and spawn the corresponding number of threads.

**WARNING:** For performance reasons each thread keeps its own agent population in memory. A large value of the NumberOfAgentsToSimulate parameter in the solution file combined with a large number of processing threads can result in high memory usage. Users should open task manager and keep an eye on memory usage to select the optimal balance between performance and memory usage, given the amount of RAM available.

### Changing model parameters

Model parameters are contained in scenario files encoded in JSON format. An example is contained in the Scenarios/ folder

### Finding the output

Output is saved in the Output/ directory. A copy of the scenario file that was used to run the model as well as a csv file containing results will be generated.

The naming convention is to start the filename with the value of the ScenarioName setting in the scenario file.

If the --saveDate switch is used then the date and time the model was run will be appended to the filename.

**WARNING:** If a file with the same name already exists it will be overwritten.

---

## Solution project structure

* *Yaabm.generic* contains the base generic classes for constructing an agent based model.
* *TestSirModel* is a simple command line application that contains a simple implementation of an agent model and shows how to apply the code in *Yaabm.generic*
* *Covid19ModelLibrary* is a class library containing the implementation of a COVID-19 agent based model.
* *RunCovidSimulation* is a command line application that will execute the model defined in *Covid19ModelLibrary*
* *Yaabm.Graph* and *GraphGen* are projects where the graph generation code that will be used to model agent contacts is being developed and tested (see below)

---

## Development status and known issues

The Covid19 model will run successfully but is not ready for real-world simulations.

The major outstanding issues are:

### Contacts and transmissions

Modelling of contacts between agents and virus transmission is not complete and produces incorrect results.

Agents are currently assigned to provinces and each province is essentially a separate bucket with no interaction with the other buckets. 
Future development plans include 

* Modelling contacts beween agents as an undirected graph, based on household structure and contact matrix input
* Modelling on a more granular level geographically, most likely based on wards
* Explicitly modelling travel between regions

### Output format

Output is currently written in CSV format, which makes it easy to import into whatever analysis software you would like. Unfortunatly the file sizes can get very large depending on the number of monte-carlo iterations.

### Model assumptions

* The model assumptions are not based on any calibration or reliable sources and the results therefore do not approximate reality.
* Some of the assumption names are based on more traditional [Compartmental models](https://en.wikipedia.org/wiki/Compartmental_models_in_epidemiology) that are based on differntial equations. The parameter names used in such models the underlying concepts do not necessarily translate to agent based models.
* Model parameters are currently specific in a JSON formatted file. Some of the aspects, such as specifying interventions (used to model changes such as non-healthcare interventions) is not user friendly at all and needs some work.

### Unit testing

Unit test coverage is currently very poor.

--- 

## References:

1. Grouped graph generation algorithm used in `MultiConfigModelGenerator` class in the *Yaabm.Graph* library is partly based on the r code in [kklot/gennet](https://github.com/kklot/gennet)

## Dependencies

This solution gratefully relies on code developed by the following projects:

* [Command Line Parser Library for CLR and NetStandard](https://github.com/commandlineparser/commandline#command-line-parser-library-for-clr-and-netstandard)
* [The Loyc Core project ](http://core.loyc.net/)
* [Math.NET Numerics](https://numerics.mathdotnet.com/)
* [Serilog](https://serilog.net/)
* [Sandwych.QuickGraph](https://github.com/oldrev/Sandwych.QuickGraph)

---

# Authors
André Bellingan

