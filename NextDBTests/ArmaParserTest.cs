using Newtonsoft.Json;
using NextDB.Parser;
using NUnit.Framework;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace NextDBTests
{
    public class ArmaParserTest
    {
        private string _testData;
        private string _unitloadout;

        [SetUp]
        public void Setup()
        {
            _testData =
                "[\"Hello there \"\"Gernal\"\"'Kenobi\",[[100,200,300],[200,300,400],[400,500,600]],true,\"true\",false,123,[[[\"\"]]]]";
            _unitloadout =
                "[[\"arifle_MXC_Holo_pointer_F\",\"\",\"acc_pointer_IR\",\"optic_Holosight\",[\"30Rnd_65x39_caseless_mag\",30],[],\"\"],[\"launch_B_Titan_short_F\",\"\",\"\",\"\",[\"Titan_AT\",1],[],\"\"],[\"hgun_P07_F\",\"\",\"\",\"\",[\"16Rnd_9x21_Mag\",17],[],\"\"],[\"U_B_CombatUniform_mcam\",[[\"ACE_fieldDressing\",1],[\"ACE_packingBandage\",1],[\"ACE_morphine\",1],[\"ACE_tourniquet\",1],[\"30Rnd_65x39_caseless_mag\",2,30]]],[\"V_PlateCarrier1_rgr\",[[\"30Rnd_65x39_caseless_mag\",3,30],[\"16Rnd_9x21_Mag\",2,17],[\"SmokeShell\",1,1],[\"SmokeShellGreen\",1,1],[\"Chemlight_green\",2,1]]],[\"B_AssaultPack_mcamo_AT\",[[\"Titan_AT\",2,1]]],\"H_HelmetB_light_desert\",\"G_Shades_Blue\",[],[\"ItemMap\",\"\",\"ItemRadio\",\"ItemCompass\",\"ItemWatch\",\"NVGoggles\"]]";
        }

        [Test]
        public void TestParser()
        {
            var parser = new ArmaParser();
            var testDataResult = parser.ReadArmaValues(_testData);

            var testDataSerialized = JsonSerializer.Serialize(testDataResult);
            
            var unitLoadoutResult = parser.ReadArmaValues(_unitloadout);

            var unitLoadoutSerialized = JsonSerializer.Serialize(unitLoadoutResult);
            Assert.Pass();
        }
    }
}