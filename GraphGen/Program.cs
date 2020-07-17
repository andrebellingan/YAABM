using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Yaabm.generic.Random;
using Yaabm.Graph.Generation;

namespace GraphGen
{
    public class Program
    {
        private static void Main()
        {
            Console.WriteLine("Hello, world");
            Console.WriteLine("This will generate the test graph for analysis");

            const bool useComplexExample = true;
            const int multiplier = 441; //4406 for about a million nodes in the complex example

            var multiLambdas = ContactMatrix(useComplexExample);
            var groupSizes = GroupSizes(useComplexExample);
            var numberOfGroups = groupSizes.Length;

            const int seed = 12345;

            var population = GeneratePopulation(numberOfGroups, groupSizes, multiplier, multiLambdas, new DefaultRandom(seed));

            TestBasic(population, seed);
            
            TestMulti(population, seed, numberOfGroups, multiLambdas);

#if DEBUG
            Console.ReadKey();
#endif
        }

        private static int[] GroupSizes(bool useComplexExample)
        {
            if (useComplexExample) return new[] { 18, 20, 20, 19, 18, 17, 16, 15, 14, 13, 12, 10, 9, 8, 7, 6, 5 };

            return new[] {10, 10};
        }

        private static void TestMulti(List<MultiConfigItem<GraphNode>> population, int seed, int numberOfGroups,
            IReadOnlyList<double[]> multiLambdas)
        {
            Console.WriteLine("Generating grouped graph");

            var graph = new TestGraph();
            var random = new DefaultRandom(seed);
            var multiGen = new MultiConfigModelGenerator<GraphNode, GraphEdge, TestGraph>(numberOfGroups, graph, random)
                {
                    Shuffle = true
                };

            var watch = new Stopwatch();

            watch.Start();
            multiGen.GenerateLinks(population, true);

            var initialTime = watch.Elapsed;

            var importedNode = new GraphNode(int.MaxValue, 0);

            var importedPopItem = new MultiConfigItem<GraphNode>
            {
                Agent = importedNode,
                GroupId = importedNode.GroupId,
                Degrees = new List<int>(numberOfGroups)
            };

            AssignRandomDegrees(multiLambdas, importedNode.GroupId, importedPopItem, random);

            var newPopulation = new List<MultiConfigItem<GraphNode>> {importedPopItem};

            multiGen.GenerateLinks(newPopulation, false);

            watch.Stop();
            Console.WriteLine($"Linking the initial population in groups took {initialTime}");
            Console.WriteLine($"Generating connections took {watch.Elapsed:g}");

            Console.WriteLine($"Generated {graph.EdgeCount} links");

            watch.Reset();
            watch.Start();
            SaveGraph(graph, "./TestGraph_Multi.sif");

            population.AddRange(newPopulation);

            SaveNodeAttributeTable(population, numberOfGroups, "./TestGraph_Multi_Nodes.csv");
            watch.Stop();
            Console.WriteLine($"Writing network to file took {watch.Elapsed:g}");
        }

        private static List<MultiConfigItem<GraphNode>> GeneratePopulation(int numberOfGroups, IReadOnlyList<int> agentsPerGroup, int multiplier,
            IReadOnlyList<double[]> multiLambdas, DefaultRandom random)
        {
            var watch = new Stopwatch();
            watch.Start();

            var population = new List<MultiConfigItem<GraphNode>>();

            for (var groupId = 0; groupId < numberOfGroups; groupId++)
            {
                for (var x = 0; x < agentsPerGroup[groupId] * multiplier; x++)
                {
                    var id = groupId * 1000000 + x;
                    var newNode = new GraphNode(id, groupId);

                    var newPopItem = new MultiConfigItem<GraphNode>
                    {
                        Agent = newNode, GroupId = groupId, Degrees = new List<int>(numberOfGroups)
                    };

                    AssignRandomDegrees(multiLambdas, groupId, newPopItem, random);
                    population.Add(newPopItem);
                }
            }

            watch.Stop();
            Console.WriteLine($"Generating population with {population.Count} agents took {watch.Elapsed:g}");

            return population;
        }

        private static void AssignRandomDegrees(IReadOnlyList<double[]> multiLambdas, int groupId, MultiConfigItem<GraphNode> newPopItem,
            DefaultRandom random)
        {
            foreach (var item in multiLambdas[groupId])
            {
                newPopItem.Degrees.Add(random.SamplePoisson(item));
            }
        }

        private static List<double[]> ContactMatrix(bool useComplexExample)
        {
            if (!useComplexExample)
            {
                return new List<double[]>
                {
                    new []{ 2.0, 1.0 },
                    new []{ 1.0, 2.0 }
                };
            }

            return new List<double[]>
            {
                new[] {3.07357486300986, 1.61384322577771, 0.854466052477136, 0.548810300929197, 0.917248990654212, 1.34363081919457, 1.41786458589074, 1.14588199850498, 0.61200818126922, 0.347069403541526, 0.362763653624432, 0.295728681825455, 0.184139605130506, 0.124758905711957, 0.0670353505444645, 0.0331421341734358, 0.0331421341734358 },
                new[] {1.40918273867567, 11.462920185846, 2.0170846792614, 0.582453214541865, 0.406087991576339, 0.942300635980314, 1.17284319234777, 1.15706448412515, 0.869363356840625, 0.390837253672632, 0.248565997492886, 0.211709403720592, 0.160264154622603, 0.100029973318278, 0.044180453748208, 0.028241768713559, 0.028241768713559 },
                new[] {0.574395147049783, 3.3806202032202, 12.5535878750154, 1.30960407334867, 0.634271621783729, 0.582429567693606, 0.689388774459721, 0.912641548430552, 0.934046841749415, 0.525921930786871, 0.275313844321476, 0.143757164669689, 0.0872443844958468, 0.0796083312781329, 0.0532469699549847, 0.0336411518719209, 0.0336411518719209 },
                new[] {0.355404927862239, 0.799885615051835, 3.74278399042537, 9.68037514075478, 2.23891283486089, 1.06417986765252, 0.682863188338205, 0.875962053404996, 0.926009295492016, 0.823154779006992, 0.415350389826107, 0.162285328387859, 0.105501848214094, 0.0547118053679996, 0.0289869441293516, 0.0167577733555426, 0.0167577733555426 },
                new[] {0.6322133908208, 0.507747872410627, 0.607537692515538, 3.86120338040018, 6.18404895011494, 2.66995665509127, 1.45872508749834, 1.12054897718051, 0.893617243099855, 0.987709574814853, 0.635683193477941, 0.307230864508766, 0.18299506397593, 0.0438765513992686, 0.0411804851841788, 0.0279529914727345, 0.0279529914727345 },
                new[] {1.16413727571011, 0.696554175180229, 0.347262579758225, 1.18161064355506, 3.15854614744702, 4.18917253416994, 2.14239678646292, 1.47987359313089, 1.09409236068005, 0.864564514121954, 0.753167088524377, 0.34369218541358, 0.252940640229582, 0.0525127789404419, 0.0216333338753183, 0.0160436606693996, 0.0160436606693996 },
                new[] {1.10013964001766, 1.5993670096766, 1.1133500557022, 0.58196246866664, 1.31511617294283, 2.13615821915027, 2.57795456078516, 1.79167637630602, 1.22402489055977, 0.863102322695671, 0.613781634302604, 0.347582902428807, 0.278919383094184, 0.0614017903062973, 0.0306502090102536, 0.0209801052742767, 0.0209801052742767 },
                new[] {0.985871714621266, 1.57423352419802, 1.25741227631875, 0.796345083990221, 0.823452579018772, 1.4681512023027, 1.7070057598714, 2.24713011180802, 1.60204036176479, 0.952980971630322, 0.616426580802482, 0.266372388244187, 0.248920635823293, 0.0955894013019137, 0.0488409384620551, 0.015512326888251, 0.015512326888251 },
                new[] {0.685914563217098, 1.1473998813565, 1.26547613840997, 1.12631832847422, 0.901834172017859, 1.12126641917934, 1.37611754531786, 1.4788806195117, 1.72770185057916, 1.1127785383127, 0.702425809337239, 0.190952862820476, 0.237829337185339, 0.0686305799018121, 0.0412801649503723, 0.0177885542479757, 0.0177885542479757 },
                new[] {0.414101017102384, 0.94266624240418, 0.980949219530756, 1.37829514174463, 0.842322891166738, 0.8836548817242, 0.98596619149587, 1.09967317150551, 1.06746456752764, 1.09680570730154, 0.664999955869187, 0.233183456963574, 0.178040804582, 0.0438942276297121, 0.035940543312634, 0.0294075666644989, 0.0294075666644989 },
                new[] {0.466897507650598, 0.886790531255483, 1.1211942394638, 1.07447132087881, 0.913037590484981, 1.07035127814318, 0.914084192408763, 0.834235956042986, 1.02494258093811, 1.02390330804401, 0.826049283607361, 0.378465688421538, 0.222341152592353, 0.0493094569180821, 0.0362437031868317, 0.0313278306042416, 0.0313278306042416 },
                new[] {0.771067103398964, 1.124762485135, 0.812847821884096, 0.817604344300595, 0.660018608883305, 0.921927901845755, 0.842623042009599, 0.587493481146276, 0.561036121450511, 0.50202075267637, 0.535867268942201, 0.453221836938471, 0.296716441510913, 0.105753785042512, 0.0416233568291634, 0.0296681370219873, 0.0296681370219873 },
                new[] {0.682547603337248, 0.794362882611455, 0.540316732158029, 0.568270265149035, 0.540235399114611, 0.783573339192616, 0.854703344985344, 0.912380808204326, 0.732621616779318, 0.622481022671457, 0.518354162503462, 0.422014004104323, 0.397199372121962, 0.159635239740919, 0.067048716487086, 0.0202973936506768, 0.0202973936506768 },
                new[] {0.396252295105928, 0.675294197536582, 0.570854429050886, 0.273521324179401, 0.273878169922848, 0.318319987002746, 0.417052587235034, 0.42486282037762, 0.318127729395692, 0.168941552147518, 0.177575274128707, 0.212050050725568, 0.186627352128173, 0.216962779127059, 0.0921289883914437, 0.0260988406185338, 0.0260988406185338 },
                new[] {0.199298742173666, 0.608283853864328, 0.593330005048174, 0.469486165912465, 0.144333509321533, 0.249003179284727, 0.206890628392943, 0.356741741409501, 0.347576264940372, 0.275084578438902, 0.221263916208712, 0.168212372308802, 0.202848027933575, 0.164365431905805, 0.171895614249103, 0.0934691354272829, 0.0934691354272829 },
                new[] {0.263182284819672, 0.394697453008653, 0.602899708717884, 0.476510810364801, 0.127504755051538, 0.131610301358101, 0.165197190573516, 0.249732363197984, 0.267203502716805, 0.241076937341318, 0.252121324829437, 0.130961300370068, 0.0793569384011506, 0.112452336747137, 0.0840418585703441, 0.0764333720470139, 0.11919852683185 },
                new[] {0.263182284819672, 0.394697453008653, 0.602899708717884, 0.476510810364801, 0.127504755051538, 0.131610301358101, 0.165197190573516, 0.249732363197984, 0.267203502716805, 0.241076937341318, 0.252121324829437, 0.130961300370068, 0.0793569384011506, 0.112452336747137, 0.0840418585703441, 0.049694676485713, 0.111065703236679 }
            };
        }

        private static void TestBasic(List<MultiConfigItem<GraphNode>> population, int seed)
        {
            Console.WriteLine("Generating ungrouped graph");

            var graph = new TestGraph();
            var random = new DefaultRandom(seed);
            var generator = new SimpleConfigModelGenerator<GraphNode, GraphEdge, TestGraph>(graph, random);

            var basicPopulation = ConvertToBasicPopulation(population);
            
            var watch = new Stopwatch();
            watch.Start();
            generator.GenerateLinks(basicPopulation, false);
            watch.Stop();
            Console.WriteLine($"Time to generate basic degree sequence graph was {watch.Elapsed:g}");

            SaveGraph(graph, "./TestGraph_Basic.sif");
        }

        private static List<Tuple<GraphNode, int>> ConvertToBasicPopulation(List<MultiConfigItem<GraphNode>> population)
        {
            var basicList = new List<Tuple<GraphNode, int>>();

            foreach (var multiItem in population)
            {
                var convertedItem = ConvertToBasicItem(multiItem);
                basicList.Add(convertedItem);
            }

            return basicList;
        }

        private static Tuple<GraphNode, int> ConvertToBasicItem(MultiConfigItem<GraphNode> multiItem)
        {
            var totalDegree = multiItem.Degrees.Sum();

            return new Tuple<GraphNode, int>(multiItem.Agent, totalDegree);
        }

        private static void SaveNodeAttributeTable(List<MultiConfigItem<GraphNode>> population, int numberOfGroups, string fileName)
        {
            using (var file = new StreamWriter(fileName))
            {
                var headings = new List<string> { "Name", "GroupId" };

                for (var g = 0; g < numberOfGroups; g++)
                    headings.Add($"DesiredDegrees_gp_{g}");

                file.WriteLine(String.Join(",", headings));

                foreach (var item in population)
                {
                    file.WriteLine($"{item.Agent.Id},{item.GroupId},{String.Join(",", item.Degrees)}");
                }
            }
        }

        private static void SaveGraph(TestGraph graph, string filename)
        {
            using (var file = new StreamWriter(filename))
            {
                foreach (var vtx in graph.Vertices)
                {
                    var vertexString = $"{vtx.Id}";

                    var adjacentEdges = graph.AdjacentEdges(vtx).ToArray();

                    if (adjacentEdges.Length > 0)
                    {
                        var otherIds = adjacentEdges.Select(p => p.OtherConnectedAgent(vtx).Id.ToString());

                        vertexString += " TypeA " + string.Join(" ", otherIds);
                    }

                    file.WriteLine(vertexString);
                }
            }
        }
    }
}
