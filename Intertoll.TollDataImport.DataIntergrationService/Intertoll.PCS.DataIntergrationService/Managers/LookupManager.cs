using Intertoll.PCS.DataIntergration.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intertoll.PCS.DataIntergrationService.Managers
{
    public static class LookupManager
    {
        public static Guid Lanes(string laneCode)
        {
            var laneGuid = new Guid();
            try
            {
                Dictionary<string, Guid> dictionary = new Dictionary<string, Guid>();
                dictionary.Add("01MS", Guid.Parse("F6B14820-7BF0-4B6D-B1D3-F6FF3C5CB9D6"));
                dictionary.Add("02MS", Guid.Parse("C5BF88ED-1326-4C3F-9F58-720B1873C044"));
                dictionary.Add("03MS", Guid.Parse("60F77936-157A-4820-8ECB-E2E5FCC46B95"));
                dictionary.Add("04MS", Guid.Parse("0B592C66-A3C9-4A61-8D2F-1B008353099F"));
                dictionary.Add("05MS", Guid.Parse("870E2702-8127-436E-BE52-7A3A9BF216E8"));
                dictionary.Add("06MS", Guid.Parse("BAE0654F-96B2-4BC1-BD18-D1FC72697CFC"));
                dictionary.Add("08MS", Guid.Parse("064256A6-A89D-4434-BFF5-DDB4106052E4"));
                dictionary.Add("09MS", Guid.Parse("26308F67-6D2A-4D65-B8C3-04E98BCF2940"));
                dictionary.Add("10MS", Guid.Parse("C7F25426-7314-42CC-9E7A-0B404AC0946A"));
                dictionary.Add("11MN", Guid.Parse("34AAD6A2-6E8C-41FA-9186-EABDBFC96BCB"));
                dictionary.Add("12MN", Guid.Parse("5E25F3EC-2BB3-44EE-8164-EE5CAD9AEAF0"));
                dictionary.Add("13MN", Guid.Parse("701D77AE-7F6B-4037-A768-18D251F09033"));
                dictionary.Add("14MN", Guid.Parse("D2A79E81-4A84-482A-ACFF-5A5FDE15334D"));
                dictionary.Add("15MN", Guid.Parse("08BB7707-40EA-459F-A18F-D7BCF971003E"));
                dictionary.Add("16MN", Guid.Parse("C3664D8C-2018-438A-872B-79F32E443D25"));
                dictionary.Add("17MN", Guid.Parse("B1902BCD-AD71-47B0-8772-CA0D1B316BFB"));
                dictionary.Add("18MN", Guid.Parse("B59F47CF-2683-4DBF-B23F-801E5503BF3A"));

                if (dictionary.ContainsKey(laneCode) == true)
                {
                    laneGuid = dictionary[laneCode];
                }
            }
            catch (Exception ex)
            {
            }
            return laneGuid;
        }

        public static Guid Classes(int classCode)
        {
            var classGuid = new Guid();
            try
            {
                Dictionary<int, Guid> dictionary = new Dictionary<int, Guid>();
                dictionary.Add(1, Guid.Parse("57C3EBAF-AD68-46CB-B048-3E9E9F1D5F74"));
                dictionary.Add(2, Guid.Parse("A9FAB369-1871-4D07-9A5A-E6923C2EEB42"));
                dictionary.Add(3, Guid.Parse("1A23A968-AF09-4B75-B973-2F010469FED2"));
                dictionary.Add(4, Guid.Parse("7ED919E9-D588-4F70-8DF1-B07B3AA65C4F"));
                dictionary.Add(5, Guid.Parse("105B2543-CF51-4F5C-9377-AF0C7F2FB2AA"));

                if (dictionary.ContainsKey(classCode) == true)
                {
                    classGuid = dictionary[classCode];
                }
            }
            catch (Exception ex)
            {
            }
            return classGuid;
        }

        public static short contextMark(string contextMark)
        {
            short contextMarkId = 0;
            try
            {
                Dictionary<string, short> dictionary = new Dictionary<string, short>();
                dictionary.Add("8E0080800D00", 1);
                dictionary.Add("8E0061800A03", 2);

                if (dictionary.ContainsKey(contextMark) == true)
                {
                    contextMarkId = dictionary[contextMark];
                }
            }
            catch (Exception ex)
            {
            }
            return contextMarkId;
        }

        public static List<PaymentGroupsMapping> PaymentGroupsMappings()
        {
            var context = new PCSDataIntergrationEntities();

            var paymentGroupsList = new List<PaymentGroupsMapping>();
            try
            {
                paymentGroupsList.AddRange(context.PaymentGroupsMappings.ToList());
            }
            catch (Exception ex)
            {
            }
            return paymentGroupsList;
        }

        public static List<PaymentMechMapping> PaymentMechsMapping()
        {
            var context = new PCSDataIntergrationEntities();

            var paymentMechList = new List<PaymentMechMapping>();
            try
            {
                paymentMechList.AddRange(context.PaymentMechMappings.ToList());
            }
            catch (Exception ex)
            {
            }
            return paymentMechList;
        }

        public static List<PaymentMethodsMapping> PaymentMethodsMapping()
        {
            var context = new PCSDataIntergrationEntities();

            var paymentMethodList = new List<PaymentMethodsMapping>();
            try
            {
                paymentMethodList.AddRange(context.PaymentMethodsMappings.ToList());
            }
            catch (Exception ex)
            {
            }
            return paymentMethodList;
        }

        public static List<PaymentTypesMapping> PaymentTypesMapping()
        {
            var context = new PCSDataIntergrationEntities();

            var paymentTypesList = new List<PaymentTypesMapping>();
            try
            {
                paymentTypesList.AddRange(context.PaymentTypesMappings.ToList());
            }
            catch (Exception ex)
            {
            }
            return paymentTypesList;
        }

        public static Guid LaneStatuses(int laneStatusCode)
        {
            var laneStatusGuid = new Guid();
            try
            {
                Dictionary<int, Guid> dictionary = new Dictionary<int, Guid>();
                dictionary.Add(1, Guid.Parse("D2710190-E72F-41FC-9ED4-C7A80E1BED30"));
                dictionary.Add(2, Guid.Parse("4731C1C5-D45C-4806-8971-EB730DB9549F"));
                dictionary.Add(3, Guid.Parse("F1714209-6048-4D31-BF9D-AF67445ABC4B"));
                dictionary.Add(4, Guid.Parse("512CAEC7-EE37-48C7-9C75-47DE9E53214A"));
                dictionary.Add(5, Guid.Parse("1BFE3F35-5EDF-48AD-AF0A-0CB1E17BF998"));
                dictionary.Add(6, Guid.Parse("2D63DB32-E7B4-4BC5-8E7E-0C3C28B17490"));
                dictionary.Add(7, Guid.Parse("A258DB4F-DCFD-493A-BEB0-E808FEAEE9E0"));

                if (dictionary.ContainsKey(laneStatusCode) == true)
                {
                    laneStatusGuid = dictionary[laneStatusCode];
                }
            }
            catch (Exception ex)
            {
            }
            return laneStatusGuid;
        }

        public static string LaneEncryptionKeys(string lanesCode)
        {
            string RSAKey = null;
            try
            {
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                dictionary.Add("01MS", "<RSAKeyValue><Modulus>ybAIVDMQF9cqNVs/S2lF3UcZcnzaCrckU2A3VBNlGIhroeyj7+UwL+3kasdZ+LNJmA2KhukyBH+7vr0RQJJ8qBkCa+MCUUaDUgAL2rwbMGUOsXmCxjqSOuVwBf5rkpGMq8bvhEG24YiJgXd6mw75SIxSi0QGm9MyQ9LAAwlouH8=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>");
                dictionary.Add("02MS", "<RSAKeyValue><Modulus>ybAIVDMQF9cqNVs/S2lF3UcZcnzaCrckU2A3VBNlGIhroeyj7+UwL+3kasdZ+LNJmA2KhukyBH+7vr0RQJJ8qBkCa+MCUUaDUgAL2rwbMGUOsXmCxjqSOuVwBf5rkpGMq8bvhEG24YiJgXd6mw75SIxSi0QGm9MyQ9LAAwlouH8=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>");
                dictionary.Add("03MS", "<RSAKeyValue><Modulus>ybAIVDMQF9cqNVs/S2lF3UcZcnzaCrckU2A3VBNlGIhroeyj7+UwL+3kasdZ+LNJmA2KhukyBH+7vr0RQJJ8qBkCa+MCUUaDUgAL2rwbMGUOsXmCxjqSOuVwBf5rkpGMq8bvhEG24YiJgXd6mw75SIxSi0QGm9MyQ9LAAwlouH8=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>");
                dictionary.Add("04MS", "<RSAKeyValue><Modulus>ybAIVDMQF9cqNVs/S2lF3UcZcnzaCrckU2A3VBNlGIhroeyj7+UwL+3kasdZ+LNJmA2KhukyBH+7vr0RQJJ8qBkCa+MCUUaDUgAL2rwbMGUOsXmCxjqSOuVwBf5rkpGMq8bvhEG24YiJgXd6mw75SIxSi0QGm9MyQ9LAAwlouH8=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>");
                dictionary.Add("05MS", "<RSAKeyValue><Modulus>ybAIVDMQF9cqNVs/S2lF3UcZcnzaCrckU2A3VBNlGIhroeyj7+UwL+3kasdZ+LNJmA2KhukyBH+7vr0RQJJ8qBkCa+MCUUaDUgAL2rwbMGUOsXmCxjqSOuVwBf5rkpGMq8bvhEG24YiJgXd6mw75SIxSi0QGm9MyQ9LAAwlouH8=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>");
                dictionary.Add("06MS", "<RSAKeyValue><Modulus>ybAIVDMQF9cqNVs/S2lF3UcZcnzaCrckU2A3VBNlGIhroeyj7+UwL+3kasdZ+LNJmA2KhukyBH+7vr0RQJJ8qBkCa+MCUUaDUgAL2rwbMGUOsXmCxjqSOuVwBf5rkpGMq8bvhEG24YiJgXd6mw75SIxSi0QGm9MyQ9LAAwlouH8=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>");
                dictionary.Add("08MS", "<RSAKeyValue><Modulus>ybAIVDMQF9cqNVs/S2lF3UcZcnzaCrckU2A3VBNlGIhroeyj7+UwL+3kasdZ+LNJmA2KhukyBH+7vr0RQJJ8qBkCa+MCUUaDUgAL2rwbMGUOsXmCxjqSOuVwBf5rkpGMq8bvhEG24YiJgXd6mw75SIxSi0QGm9MyQ9LAAwlouH8=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>");
                dictionary.Add("09MS", "<RSAKeyValue><Modulus>ybAIVDMQF9cqNVs/S2lF3UcZcnzaCrckU2A3VBNlGIhroeyj7+UwL+3kasdZ+LNJmA2KhukyBH+7vr0RQJJ8qBkCa+MCUUaDUgAL2rwbMGUOsXmCxjqSOuVwBf5rkpGMq8bvhEG24YiJgXd6mw75SIxSi0QGm9MyQ9LAAwlouH8=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>");
                dictionary.Add("10MS", "<RSAKeyValue><Modulus>ybAIVDMQF9cqNVs/S2lF3UcZcnzaCrckU2A3VBNlGIhroeyj7+UwL+3kasdZ+LNJmA2KhukyBH+7vr0RQJJ8qBkCa+MCUUaDUgAL2rwbMGUOsXmCxjqSOuVwBf5rkpGMq8bvhEG24YiJgXd6mw75SIxSi0QGm9MyQ9LAAwlouH8=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>");
                dictionary.Add("11MN", "<RSAKeyValue><Modulus>ybAIVDMQF9cqNVs/S2lF3UcZcnzaCrckU2A3VBNlGIhroeyj7+UwL+3kasdZ+LNJmA2KhukyBH+7vr0RQJJ8qBkCa+MCUUaDUgAL2rwbMGUOsXmCxjqSOuVwBf5rkpGMq8bvhEG24YiJgXd6mw75SIxSi0QGm9MyQ9LAAwlouH8=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>");
                dictionary.Add("12MN", "<RSAKeyValue><Modulus>ybAIVDMQF9cqNVs/S2lF3UcZcnzaCrckU2A3VBNlGIhroeyj7+UwL+3kasdZ+LNJmA2KhukyBH+7vr0RQJJ8qBkCa+MCUUaDUgAL2rwbMGUOsXmCxjqSOuVwBf5rkpGMq8bvhEG24YiJgXd6mw75SIxSi0QGm9MyQ9LAAwlouH8=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>");
                dictionary.Add("13MN", "<RSAKeyValue><Modulus>ybAIVDMQF9cqNVs/S2lF3UcZcnzaCrckU2A3VBNlGIhroeyj7+UwL+3kasdZ+LNJmA2KhukyBH+7vr0RQJJ8qBkCa+MCUUaDUgAL2rwbMGUOsXmCxjqSOuVwBf5rkpGMq8bvhEG24YiJgXd6mw75SIxSi0QGm9MyQ9LAAwlouH8=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>");
                dictionary.Add("14MN", "<RSAKeyValue><Modulus>ybAIVDMQF9cqNVs/S2lF3UcZcnzaCrckU2A3VBNlGIhroeyj7+UwL+3kasdZ+LNJmA2KhukyBH+7vr0RQJJ8qBkCa+MCUUaDUgAL2rwbMGUOsXmCxjqSOuVwBf5rkpGMq8bvhEG24YiJgXd6mw75SIxSi0QGm9MyQ9LAAwlouH8=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>");
                dictionary.Add("15MN", "<RSAKeyValue><Modulus>ybAIVDMQF9cqNVs/S2lF3UcZcnzaCrckU2A3VBNlGIhroeyj7+UwL+3kasdZ+LNJmA2KhukyBH+7vr0RQJJ8qBkCa+MCUUaDUgAL2rwbMGUOsXmCxjqSOuVwBf5rkpGMq8bvhEG24YiJgXd6mw75SIxSi0QGm9MyQ9LAAwlouH8=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>");
                dictionary.Add("16MN", "<RSAKeyValue><Modulus>ybAIVDMQF9cqNVs/S2lF3UcZcnzaCrckU2A3VBNlGIhroeyj7+UwL+3kasdZ+LNJmA2KhukyBH+7vr0RQJJ8qBkCa+MCUUaDUgAL2rwbMGUOsXmCxjqSOuVwBf5rkpGMq8bvhEG24YiJgXd6mw75SIxSi0QGm9MyQ9LAAwlouH8=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>");
                dictionary.Add("17MN", "<RSAKeyValue><Modulus>ybAIVDMQF9cqNVs/S2lF3UcZcnzaCrckU2A3VBNlGIhroeyj7+UwL+3kasdZ+LNJmA2KhukyBH+7vr0RQJJ8qBkCa+MCUUaDUgAL2rwbMGUOsXmCxjqSOuVwBf5rkpGMq8bvhEG24YiJgXd6mw75SIxSi0QGm9MyQ9LAAwlouH8=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>");
                dictionary.Add("18MN", "<RSAKeyValue><Modulus>ybAIVDMQF9cqNVs/S2lF3UcZcnzaCrckU2A3VBNlGIhroeyj7+UwL+3kasdZ+LNJmA2KhukyBH+7vr0RQJJ8qBkCa+MCUUaDUgAL2rwbMGUOsXmCxjqSOuVwBf5rkpGMq8bvhEG24YiJgXd6mw75SIxSi0QGm9MyQ9LAAwlouH8=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>");

                if (dictionary.ContainsKey(lanesCode) == true)
                {
                    RSAKey = dictionary[lanesCode];
                }
            }
            catch (Exception ex)
            {
            }
            return RSAKey;
        }

        public static List<uspIncidentTypesGet_Result> IncidentTypes()
        {
            var incidentTypeList = new List<uspIncidentTypesGet_Result>();
            try
            {
                var context = new PCSDataIntergrationEntities();
                incidentTypeList.AddRange(context.uspIncidentTypesGet());
            }
            catch (Exception ex)
            {
            }
            return incidentTypeList;
        }

        //public static Guid? IncidentTypesMapping(string incidentCode)
        //{

        //    string incidentType = string.Empty;
        //    try
        //    {
        //        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        //        dictionary.Add("0", "B6AB99E8-5DA4-4C33-9EDD-08298A1A2104");
        //        dictionary.Add("1", "036C8AE2-E700-DF11-AD4B-001517C991CF");
        //        dictionary.Add("2", "046C8AE2-E700-DF11-AD4B-001517C991CF");
        //        dictionary.Add("3", "056C8AE2-E700-DF11-AD4B-001517C991CF");
        //        dictionary.Add("4", "066C8AE2-E700-DF11-AD4B-001517C991CF");
        //        dictionary.Add("5", "076C8AE2-E700-DF11-AD4B-001517C991CF");
        //        dictionary.Add("6", "086C8AE2-E700-DF11-AD4B-001517C991CF");
        //        dictionary.Add("7", "096C8AE2-E700-DF11-AD4B-001517C991CF");
        //        dictionary.Add("8", "0A6C8AE2-E700-DF11-AD4B-001517C991CF");
        //        dictionary.Add("9", "0B6C8AE2-E700-DF11-AD4B-001517C991CF");
        //        dictionary.Add("10", "0C6C8AE2-E700-DF11-AD4B-001517C991CF");
        //        dictionary.Add("11", "0D6C8AE2-E700-DF11-AD4B-001517C991CF");
        //        dictionary.Add("12", "0E6C8AE2-E700-DF11-AD4B-001517C991CF");
        //        dictionary.Add("13", "0F6C8AE2-E700-DF11-AD4B-001517C991CF");
        //        dictionary.Add("14", "106C8AE2-E700-DF11-AD4B-001517C991CF");
        //        dictionary.Add("15", "116C8AE2-E700-DF11-AD4B-001517C991CF");
        //        dictionary.Add("16", "126C8AE2-E700-DF11-AD4B-001517C991CF");
        //        dictionary.Add("17", "136C8AE2-E700-DF11-AD4B-001517C991CF");
        //        dictionary.Add("18", "146C8AE2-E700-DF11-AD4B-001517C991CF");
        //        dictionary.Add("19", "156C8AE2-E700-DF11-AD4B-001517C991CF");
        //        dictionary.Add("20", "166C8AE2-E700-DF11-AD4B-001517C991CF");
        //        dictionary.Add("21", "176C8AE2-E700-DF11-AD4B-001517C991CF");
        //        dictionary.Add("22", "186C8AE2-E700-DF11-AD4B-001517C991CF");
        //        dictionary.Add("23", "196C8AE2-E700-DF11-AD4B-001517C991CF");
        //        dictionary.Add("24", "1A6C8AE2-E700-DF11-AD4B-001517C991CF");
        //        dictionary.Add("25", "356C8AE2-E700-DF11-AD4B-001517C991CF");
        //        dictionary.Add("26", "1B6C8AE2-E700-DF11-AD4B-001517C991CF");
        //        dictionary.Add("27", "1C6C8AE2-E700-DF11-AD4B-001517C991CF");
        //        dictionary.Add("28", "366C8AE2-E700-DF11-AD4B-001517C991CF");
        //        dictionary.Add("29", "376C8AE2-E700-DF11-AD4B-001517C991CF");
        //        dictionary.Add("30", "386C8AE2-E700-DF11-AD4B-001517C991CF");
        //        dictionary.Add("31", "396C8AE2-E700-DF11-AD4B-001517C991CF");
        //        dictionary.Add("32", "3A6C8AE2-E700-DF11-AD4B-001517C991CF");
        //        dictionary.Add("33", "266C8AE2-E700-DF11-AD4B-001517C991CF");
        //        dictionary.Add("34", "1D6C8AE2-E700-DF11-AD4B-001517C991CF");
        //        dictionary.Add("35", "276C8AE2-E700-DF11-AD4B-001517C991CF");
        //        dictionary.Add("37", "286C8AE2-E700-DF11-AD4B-001517C991CF");
        //        dictionary.Add("38", "296C8AE2-E700-DF11-AD4B-001517C991CF");
        //        dictionary.Add("39", "2A6C8AE2-E700-DF11-AD4B-001517C991CF");
        //        dictionary.Add("40", "2B6C8AE2-E700-DF11-AD4B-001517C991CF");
        //        dictionary.Add("41", "1E6C8AE2-E700-DF11-AD4B-001517C991CF");
        //        dictionary.Add("42", "1F6C8AE2-E700-DF11-AD4B-001517C991CF");
        //        dictionary.Add("43", "206C8AE2-E700-DF11-AD4B-001517C991CF");
        //        dictionary.Add("44", "216C8AE2-E700-DF11-AD4B-001517C991CF");
        //        dictionary.Add("45", "3B6C8AE2-E700-DF11-AD4B-001517C991CF");
        //        dictionary.Add("46", "226C8AE2-E700-DF11-AD4B-001517C991CF");
        //        dictionary.Add("47", "236C8AE2-E700-DF11-AD4B-001517C991CF");
        //        dictionary.Add("48", "2C6C8AE2-E700-DF11-AD4B-001517C991CF");
        //        dictionary.Add("49", "2D6C8AE2-E700-DF11-AD4B-001517C991CF");
        //        dictionary.Add("50", "2E6C8AE2-E700-DF11-AD4B-001517C991CF");
        //        dictionary.Add("51", "2F6C8AE2-E700-DF11-AD4B-001517C991CF");
        //        dictionary.Add("52", "306C8AE2-E700-DF11-AD4B-001517C991CF");
        //        dictionary.Add("53", "316C8AE2-E700-DF11-AD4B-001517C991CF");
        //        dictionary.Add("54", "246C8AE2-E700-DF11-AD4B-001517C991CF");
        //        dictionary.Add("55", "256C8AE2-E700-DF11-AD4B-001517C991CF");
        //        dictionary.Add("56", "326C8AE2-E700-DF11-AD4B-001517C991CF");
        //        dictionary.Add("57", "336C8AE2-E700-DF11-AD4B-001517C991CF");
        //        dictionary.Add("58", "346C8AE2-E700-DF11-AD4B-001517C991CF");
        //        dictionary.Add("59", "66D07173-ADDF-E111-8706-C82A14062982");
        //        dictionary.Add("60", "E38FD47E-ADDF-E111-8706-C82A14062982");
        //        dictionary.Add("61", "02F51487-ADDF-E111-8706-C82A14062982");
        //        dictionary.Add("62", "B5BF249B-ADDF-E111-8706-C82A14062982");
        //        dictionary.Add("63", "4FF9BDA6-ADDF-E111-8706-C82A14062982");
        //        dictionary.Add("64", "5B5C35B0-ADDF-E111-8706-C82A14062982");
        //        dictionary.Add("65", "427406C7-ADDF-E111-8706-C82A14062982");
        //        dictionary.Add("66", "84E9B6D0-ADDF-E111-8706-C82A14062982");
        //        dictionary.Add("67", "338455DA-ADDF-E111-8706-C82A14062982");
        //        dictionary.Add("68", "26E39FE3-ADDF-E111-8706-C82A14062982");
        //        dictionary.Add("69", "1192DBEE-ADDF-E111-8706-C82A14062982");
        //        dictionary.Add("70", "13D0B6F9-ADDF-E111-8706-C82A14062982");
        //        dictionary.Add("71", "65F54D06-AEDF-E111-8706-C82A14062982");
        //        dictionary.Add("72", "E9135913-AEDF-E111-8706-C82A14062982");
        //        dictionary.Add("73", "F7FBBF1C-AEDF-E111-8706-C82A14062982");
        //        dictionary.Add("74", "6F568B24-AEDF-E111-8706-C82A14062982");
        //        dictionary.Add("75", "4F19482D-AEDF-E111-8706-C82A14062982");
        //        dictionary.Add("80", "988787B3-45E4-4845-AE6E-3D392ECEA75D");
        //        dictionary.Add("81", "31B5401D-AEDF-4807-8006-76CAD267ECD4");
        //        dictionary.Add("82", "B3C59AEC-C343-401F-AC36-855811BBAC97");
        //        dictionary.Add("84", "D121C4ED-3998-4E3F-A126-8C8D944BD0D8");
        //        dictionary.Add("87", "B03BE6C0-92D1-462A-B8A1-C463E88752E1");
        //        dictionary.Add("92", "CF06DED5-5048-46EC-AB28-902E95BBD8F5");
        //        dictionary.Add("93", "81FCAD31-5A18-4666-89BC-D5E8895E15C3");
        //        dictionary.Add("94", "7F9746B5-9854-4AA5-A982-E18E54FE2729");
        //        dictionary.Add("1000", "A0EDDE4E-AB26-DF11-BA8B-001517C991CF");
        //        dictionary.Add("1001", "5047FF60-AB26-DF11-BA8B-001517C991CF");
        //        dictionary.Add("1002", "70A24C7A-AB26-DF11-BA8B-001517C991CF");
        //        dictionary.Add("1003", "3001A02D-AE26-DF11-BA8B-001517C991CF");
        //        dictionary.Add("1004", "D1E31F77-BC26-DF11-BA8B-001517C991CF");
        //        dictionary.Add("1005", "B0872692-BC26-DF11-BA8B-001517C991CF");
        //        dictionary.Add("1006", "000BC09D-BC26-DF11-BA8B-001517C991CF");
        //        dictionary.Add("1007", "D5CC02E2-ED9E-DF11-A601-001517DD2D44");
        //        dictionary.Add("5001", "35064552-AEDF-E111-8706-C82A14062982");
        //        dictionary.Add("5002", "2062AF5A-AEDF-E111-8706-C82A14062982");
        //        dictionary.Add("5003", "9A60B261-AEDF-E111-8706-C82A14062982");
        //        dictionary.Add("5004", "8E4ADB6C-AEDF-E111-8706-C82A14062982");
        //        dictionary.Add("5005", "346E8874-AEDF-E111-8706-C82A14062982");
        //        dictionary.Add("5006", "356E8874-AEDF-E111-8706-C82A14062982");
        //        dictionary.Add("5007", "6BA4C782-AEDF-E111-8706-C82A14062982");
        //        dictionary.Add("5008", "5BF3438A-AEDF-E111-8706-C82A14062982");
        //        dictionary.Add("5009", "59C10497-AEDF-E111-8706-C82A14062982");
        //        dictionary.Add("5010", "F20D089E-AEDF-E111-8706-C82A14062982");
        //        dictionary.Add("5011", "A35DBCA5-AEDF-E111-8706-C82A14062982");
        //        dictionary.Add("5012", "F1758DAC-AEDF-E111-8706-C82A14062982");
        //        dictionary.Add("5013", "4DD4A6B2-AEDF-E111-8706-C82A14062982");
        //        dictionary.Add("5014", "90632CB9-AEDF-E111-8706-C82A14062982");
        //        dictionary.Add("5015", "BCE785C0-AEDF-E111-8706-C82A14062982");
        //        dictionary.Add("5016", "921B08C7-AEDF-E111-8706-C82A14062982");
        //        dictionary.Add("5017", "09CCACCD-AEDF-E111-8706-C82A14062982");
        //        dictionary.Add("5018", "509516DC-AEDF-E111-8706-C82A14062982");
        //        dictionary.Add("5019", "204135E4-AEDF-E111-8706-C82A14062982");
        //        dictionary.Add("5020", "FDADACEB-AEDF-E111-8706-C82A14062982");
        //        dictionary.Add("5021", "FEADACEB-AEDF-E111-8706-C82A14062982");
        //        dictionary.Add("5022", "4EB687F5-AEDF-E111-8706-C82A14062982");
        //        dictionary.Add("5023", "4FB687F5-AEDF-E111-8706-C82A14062982");
        //        dictionary.Add("5024", "E1F99A03-AFDF-E111-8706-C82A14062982");
        //        dictionary.Add("5025", "E2F99A03-AFDF-E111-8706-C82A14062982");
        //        dictionary.Add("5026", "2AD55E10-AFDF-E111-8706-C82A14062982");
        //        dictionary.Add("5027", "0A51A2F5-B2DF-E111-8706-C82A14062982");
        //        dictionary.Add("5028", "0AC85655-231C-E211-97C6-E0F8473BA773");
        //        dictionary.Add("5029", "D05B3362-231C-E211-97C6-E0F8473BA773");
        //        dictionary.Add("5030", "B67D5432-7B2D-4955-ACE1-BA21F56F155F");
        //        dictionary.Add("5031", "AB433CA4-EEE2-434E-A4B3-3A73550FEE02");
        //        dictionary.Add("5032", "28592B49-5215-417A-A6A2-10B4CFEEF7CE");
        //        dictionary.Add("5033", "30F4A098-245B-46C2-BBC4-EA104F546E56");
        //        dictionary.Add("5034", "E4898572-0076-49F7-830F-88E37ABE6556");
        //        dictionary.Add("5035", "0F2197B7-0D82-42C6-B772-DEBAA8624AD5");
        //        dictionary.Add("5036", "286A5A3B-B496-46B5-B8D7-033CCE9D32E9");
        //        dictionary.Add("5037", "703ABBC7-7FD2-4161-8081-2D7223C8169A");
        //        dictionary.Add("5038", "FDDC4B3C-84F0-4B04-ABF8-C19BD58C6A86");
        //        dictionary.Add("5039", "37138ABA-A01B-442A-86AC-1B060871C98A");
        //        dictionary.Add("5040", "0606549B-7DFF-4219-9C1A-EC57E479B3B0");
        //        dictionary.Add("5041", "3A170926-5689-4BEB-B6F1-E5F93452553F");
        //        dictionary.Add("5042", "A7D5275E-B5DD-40E1-9EAD-FAE96B526E6B");
        //        dictionary.Add("5043", "D0700273-3E27-40DB-962C-A3DC44AF37BA");
        //        dictionary.Add("5044", "28BC43D4-C88F-4F02-A5C7-1285F0530706");
        //        dictionary.Add("5045", "A7020A3C-62F1-478E-A74D-D66BA82F08E0");
        //        dictionary.Add("5046", "9EA66238-B66B-4BCE-A5CD-7ED40267D31C");
        //        dictionary.Add("5047", "06B473E6-E7A8-45E4-93BD-B3033CDE7C88");
        //        dictionary.Add("5048", "2157A470-B777-424C-B4A5-5743459B90E5");
        //        dictionary.Add("5061", "658F8C01-1BD0-4A52-9870-4482AFBD4655");
        //        dictionary.Add("5062", "2B12CE23-8C6B-4732-8C7A-B9187B0B538E");
        //        dictionary.Add("5063", "161A9328-BE23-4C29-B9D6-9C506C9F9F4F");
        //        dictionary.Add("5064", "7E3C436E-29B3-4E85-BA69-70830ECABCD1");
        //        dictionary.Add("5065", "9420798F-433B-4245-AF0B-320C8FD39B7A");
        //        dictionary.Add("5066", "D73B84D4-5E2A-4EA2-9F9D-0916E941B206");
        //        dictionary.Add("5067", "2D199674-B098-484A-A861-BB0B0E8D1862");
        //        dictionary.Add("5068", "755DE521-8DA3-4E99-948C-C87C71FAFBF8");
        //        dictionary.Add("5069", "58640300-C388-43C1-B545-BC656CBE3952");
        //        dictionary.Add("5100", "7AA9E945-192E-E511-BB65-001917BD1D46");
        //        dictionary.Add("10000", "CA60A87F-8F5D-DF11-934B-001517C991CF");

        //        ////Add 300 range

        //        //dictionary.Add("300", "B6AB99E8-5DA4-4C33-9EDD-08298A1A2104");//BatteryCharging = undefined on intertoll
        //        dictionary.Add("301", "FDDC4B3C-84F0-4B04-ABF8-C19BD58C6A86"); //AVC Critical Mode Start
        //        dictionary.Add("302", "37138ABA-A01B-442A-86AC-1B060871C98A");//AVC Critical Mode Ended
        //        dictionary.Add("303", "0606549B-7DFF-4219-9C1A-EC57E479B3B0");//AVC Serious Mode Start
        //        dictionary.Add("304", "3A170926-5689-4BEB-B6F1-E5F93452553F");//AVC Serious Mode Ended
        //        //dictionary.Add("305", "B6AB99E8-5DA4-4C33-9EDD-08298A1A2104");//OHLSToGreen
        //        //dictionary.Add("306", "B6AB99E8-5DA4-4C33-9EDD-08298A1A2104");//OHLSToRed
        //        //dictionary.Add("307", "B6AB99E8-5DA4-4C33-9EDD-08298A1A2104");//CashLimitReached
        //        //dictionary.Add("308", "B6AB99E8-5DA4-4C33-9EDD-08298A1A2104");//AutoTariffUpdate
        //        dictionary.Add("309", "509516DC-AEDF-E111-8706-C82A14062982");//CancelledClassSelection
        //        dictionary.Add("310", "2AD55E10-AFDF-E111-8706-C82A14062982");//ManualKeyEntry
        //        //dictionary.Add("311", "B6AB99E8-5DA4-4C33-9EDD-08298A1A2104");//QueueLineBreached
        //        //dictionary.Add("312", "B6AB99E8-5DA4-4C33-9EDD-08298A1A2104");//QueueLineBreachCleared 
        //        //dictionary.Add("313", "B6AB99E8-5DA4-4C33-9EDD-08298A1A2104");//QLSCameraFailureStart
        //        //dictionary.Add("314", "B6AB99E8-5DA4-4C33-9EDD-08298A1A2104");//QLSCameraFailureStop
        //        //dictionary.Add("315", "B6AB99E8-5DA4-4C33-9EDD-08298A1A2104");//ApplicationNotRresponding
        //        //dictionary.Add("316", "B6AB99E8-5DA4-4C33-9EDD-08298A1A2104");//ApplicationResponding
        //        //dictionary.Add("317", "B6AB99E8-5DA4-4C33-9EDD-08298A1A2104");//TCCUserLogin
        //        //dictionary.Add("318", "B6AB99E8-5DA4-4C33-9EDD-08298A1A2104");//TCCUserLogout
        //        //dictionary.Add("319", "B6AB99E8-5DA4-4C33-9EDD-08298A1A2104");//StartAVCCommunicationFail
        //        //dictionary.Add("320", "B6AB99E8-5DA4-4C33-9EDD-08298A1A2104");//EndAVCCommunicationFail
        //        //dictionary.Add("321", "B6AB99E8-5DA4-4C33-9EDD-08298A1A2104");//PanicButtonPressed
        //        dictionary.Add("322", "658F8C01-1BD0-4A52-9870-4482AFBD4655");//ExemptPassage

        //        if (dictionary.ContainsKey(incidentCode) == true)
        //        {
        //            incidentType = dictionary[incidentCode];
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //    if (string.IsNullOrWhiteSpace(incidentType)) 
        //        return null;
        //    else
        //        return Guid.Parse(incidentType);
        //}

        public static List<TariffMapping> TariffMapping()
        {
            var context = new PCSDataIntergrationEntities();

            var tariffList = new List<TariffMapping>();
            try
            {
                tariffList.AddRange(context.TariffMappings.ToList());
            }
            catch (Exception ex)
            {
            }
            return tariffList;
        }


        public static List<Lane> LaneIPAdresses()
        {
            var lanes = new List<Lane>();
            try
            {
                var context = new PCSDataIntergrationEntities();
                lanes.AddRange(context.Lanes);
            }
            catch (Exception ex)
            {
            }
            return lanes;
        }
    }   
   
}
