using xsy.likes.pwmd;

namespace XsyRlx
{
    public class RlxData : pwapi
    {
        private const string UserId = "13460279281";
        private const string Pwd = "123456";
        private const string Code = "k9eLHPNA";
        private const string TokenClientId = "e0b228cf2d9cf5e89a893742cb416af6";
        private const string TokenClientSecret = "69eb22bdd3a75175d6e3f8f8fcf692b1";
        private const string TokenRedirectUrl = "https://api.rlxoa.com";


        public RlxData() : base(UserId, Pwd, Code, TokenClientId, TokenClientSecret, TokenRedirectUrl)
        {

        }

        public string infoShow()
        {

            return CustomerInterfaceSelect();
        }

    }
}
