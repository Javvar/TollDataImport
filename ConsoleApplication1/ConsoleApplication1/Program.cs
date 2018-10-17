using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intertoll.Encryption;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            Intertoll.Encryption.RSADecryption d = new RSADecryption(1024, "<RSAKeyValue><Modulus>ybAIVDMQF9cqNVs/S2lF3UcZcnzaCrckU2A3VBNlGIhroeyj7+UwL+3kasdZ+LNJmA2KhukyBH+7vr0RQJJ8qBkCa+MCUUaDUgAL2rwbMGUOsXmCxjqSOuVwBf5rkpGMq8bvhEG24YiJgXd6mw75SIxSi0QGm9MyQ9LAAwlouH8=</Modulus><Exponent>AQAB</Exponent><P>+FGltO8/Vr2lMYeevenSKV5LCcV0nTHL82isnOJkqSoPXsfOtfLrn8fiZgOMFBKp7YYa4UBr8ARd1SZyocypJw==</P><Q>z+0gu+vTHG+VJvsR2mhS4b8cnfF5OBZF3WvmlNZ7h5++RPTF87AGsrrPibegYjdSbc8DzBfopZS9EaMjC9mc6Q==</Q><DP>0sMx+21pfC0A7hYnJRg1ucj/ta5zeQyQB+wbPhllyLMbUp4SlBo35WkZfu6Z0Vu6ARFm9TLhqll6bvTwLOZx1w==</DP><DQ>hQRIOrBFIB3qJ0PbSgQPccfXQNuoFs945owOuQz1ffAdwvNsZ9cmkdSczJeijPKjUwhqf3iUJsmeotfgpcYTcQ==</DQ><InverseQ>FoKL3uD7oMp7f9IUQyYdMp6kj5NZyDPG1YizmI7b1d/sJ7bAms4hNi7GCpm2aevy+YntP2iPwtvcNyQSq3CFtA==</InverseQ><D>ATcxZaiFIU+nrnilUgQJasMB8UG1Yrkl2+bJjpUrV9GCrJArkMeV2cQaS1Z2QVx7RhJW0mdXaNWIQScMkpIPdEUqWLXubJ+DdlMJLZrXnFznamV8UAqUPtkj8rYvIYoZwpL9vJnzDM8LjXao/ban39KvxFbIKB/D6444hwI1ktE=</D></RSAKeyValue>");

            var dd =
                d.Decrypt(
                    "YZZzPDnlXY7l8fh8yeYTTYlz9qV/Ejlt+d5zPnawK7Q072bKM7oMzT/FHLa35YDzQE0C9VJ67jRK9IcT3MgZI77Jovlf4+0HRTvX3BxzqVdi3L4xbgOSuVac9P6LsWPQT5XLV9IJcjzCBu/uXlPuX6ZCfDi/NIqOQidHXWS7dIo=feKE0jOGD44OsHXJ1T5pMCDiDnzr2kNvm0dfANVtyTkZVOrnWxLSXyFSL1pvT08TPMQf/hG6z52mtqFAekkZRbVk9GrV48TCU1riDqYbImLHsI461IKQVoHap+Sqng+Qx8SQoH6HUDy7OjsldjDkbYiGG/DA1CgJNx/na9RAeDI=");
            Console.WriteLine(dd.ToString());
        }
    }
}
