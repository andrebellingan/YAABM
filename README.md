# Yet Another Agent-Based Model (YAABM)

This is an agent based model that is being developed to model the impact of the COVID-19 pandemic in South Africa.

---

## Getting started

### System requirements
* .NET Core 3.1 runtimes
* Sufficient RAM given the size of the simulation and the number of processing threads (see below)

### Compiling the code

1. Download or clone the source code
2. Compiling
	* It is recommended to open the solution in Visual Studio 2019 and build the projected from there (The Visual Studio Community editions are free to download for non-commercial use). Visual Studio code works too.
	* Alternatively you can download the [.NET Core 3.1 SDK](https://dotnet.microsoft.com/download/dotnet-core/3.1) and use ```dotnet build YAABM.sln``` from the command line
3. Note that the nuget package manager may indicate that there is an update available for the "CommandLineArgumentsParser" package. Due to a bug in version 3.0.20 of the package when handling default argument values you should not upgrade to this specific version.

### Running the model

To run the model you need to execute **RunCovidSimulation** from the command line.

The application has the following command line arguments:

	-n, --iterations  [Required] How many Monte Carlo iterations to simulate

	-s, --scenario    [Required] The path to the scenario settings file

	-t, --threads     [Optional] the number of parallel threads to use. 

	-d, --saveDate    [Optional] Switch to add the date and time to output file names

	-c, --contactGraphs [Optional] Switch to save the contact graph used in each iteration to a GraphML file

	-g, --geoDetail [Optional] Set the level of geographic detail in the output. Default is National

example: `RunCovidSimulation.exe -n 100 -s ./Scenarios/Baseline_scenario.json -t 4 -c -g Province`

The valid values for the -g option, in increasing level of detail are:
* National
* Province
* DistrictMunicipality
* LocalMunicipality
* Ward

### Threading
The default value for the number of threads is `-t 1`. Running single threaded is highly recommended for debugging. 

Your mileage may vary but multi-threading with values greater than `-t 4` will not significanly improve runtimes and very large values  may actually result in sub-optimal performance.

Setting `-t 0` will detect the number of logical cores on the system and spawn the corresponding number of threads. For the reasons state above this may be counter-productive on systems with a large number of cores and is therefore not recommended.

**WARNING:** For performance reasons each thread keeps its own agent population in memory. A large value of the NumberOfAgents parameter in the scenario file combined with a large number of processing threads can result in high memory usage. Users should open task manager and keep an eye on memory usage to select the optimal balance between performance and memory usage, given the amount of RAM available.

### Changing model parameters

Model parameters are contained in scenario files encoded in JSON format. An example is contained in the Scenarios/ folder

### Finding the output

Output is saved in the Output/ directory. A copy of the scenario file that was used to run the model as well as a csv file containing results will be generated.

The naming convention is to start the filename with the value of the ScenarioName setting in the scenario file.

If the --saveDate switch is used then the date and time the model was run will be appended to the file name.

**WARNING:** If a file with the same name already exists it will be overwritten.

---

## Solution project structure

* *Yaabm.generic* contains the base generic classes for constructing an agent based model.
* *TestSirModel* is a simple command line application that contains a simple implementation of an agent model and shows how to apply the code in *Yaabm.generic*. This is also used to compare the results to a theoretical SEIR model with the same parameters.
* *Covid19ModelLibrary* is a class library containing the implementation of a COVID-19 agent based model.
* *RunCovidSimulation* is a command line application that will execute the model defined in *Covid19ModelLibrary*
* *Yaabm.Graph* contains the code used for modelling connections between agents as an undirected graph

---

## Development status and known issues

The Covid19 model will run successfully but is not ready for real-world simulations.

The major outstanding issues are:

### Output format

Output is currently written in CSV format, which makes it easy to import into whatever analysis software you would like. Depending on the geographic detail level and the number of iterations the file sizes can get very large.

### Model assumptions

* The model assumptions are not based on any calibration or reliable sources and the results therefore do not approximate any realistic outcomes.
* Some of the assumption names are based on more traditional [Compartmental models](https://en.wikipedia.org/wiki/Compartmental_models_in_epidemiology) that are based on differential equations. The parameter names used in such models the underlying concepts do not necessarily translate to agent based models.
* Model parameters are currently specific in a JSON formatted file. Some of the aspects, such as specifying interventions (used to model changes such as non-healthcare interventions) is not user friendly at all and needs some work.

### Unit testing

Unit test coverage is currently very poor.

--- 

## References:

1. Contact graph generation code is based on the [SABCOM model](https://github.com/blackrhinoabm/sabcom)

2. The multi state disease model is based on the Actuarial Society of South Africa [SEIR model](https://github.com/Percept-Health-Solve/seir-model)

3. The sample contact matrices are taken from Prem K, Cook AR, Jit M (2017) Projecting social contact matrices in 152 countries using contact surveys and demographic data. PLoS Comput Biol 13(9): e1005697. https://doi.org/10.1371/journal.pcbi.1005697

4. The sample population data is based on South African Census Community Profiles 2011, created by [Statistics South Africa](http://www.statssa.gov.za/?page_id=3839) and retrieved from the [DataFirst](https://www.datafirst.uct.ac.za/) service

5. The sample travel matrix is based on National Household Travel Survey 2013, created by [Statistics South Africa](https://www.statssa.gov.za/publications/P0320/P03202013.pdf) and retrieved from the [DataFirst](https://www.datafirst.uct.ac.za/) service

## Dependencies

This solution gratefully relies on code developed by the following projects:

* [Command Line Parser Library for CLR and NetStandard](https://github.com/commandlineparser/commandline#command-line-parser-library-for-clr-and-netstandard)
* [CSVHelper](https://joshclose.github.io/CsvHelper/)
* [The Loyc Core project ](http://core.loyc.net/)
* [Math.NET Numerics](https://numerics.mathdotnet.com/)
* [QuickGraph.NETStandard](https://github.com/deepakkumar1984/QuickGraph.NETStandard)
* [Serilog](https://serilog.net/)

# Disclaimer

This code is a work in progress. The possibility of errors and bugs cannot be ruled out and the use of this code is therefore at your own risk.
