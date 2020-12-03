using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackAndTrace.Helpers;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace TrackAndTrace.ViewModels
{
    public class AuthViewModel
    {

        private static AuthViewModel _instance;
        public static AuthViewModel Instance
        {
            get
            {
                if (_instance==null)
                    return new AuthViewModel();
                return _instance;
            }
        }
        
        public string EncodePassword(string _p)
        {
            KeyDerivationAlgorithmProvider provider = KeyDerivationAlgorithmProvider.OpenAlgorithm(KeyDerivationAlgorithmNames.Pbkdf2Sha1);
            IBuffer buffer = CryptographicBuffer.ConvertStringToBinary(_p, BinaryStringEncoding.Utf8);

            IBuffer salt = CryptographicBuffer.ConvertStringToBinary("xxxxxxxxx", BinaryStringEncoding.Utf8);
            KeyDerivationParameters parameters = KeyDerivationParameters.BuildForPbkdf2(salt, 10000);

            CryptographicKey key = provider.CreateKey(buffer);
            IBuffer derivedKey = CryptographicEngine.DeriveKeyMaterial(key, parameters, 16);
            var hash = CryptographicBuffer.EncodeToHexString(derivedKey);

            return hash;
        }


        public async Task Authorize(string _username, string _password)
        {
            var hashedPass = EncodePassword(_password);

            var validniKorisnik = await App.UposleniciRepository.GetList(u => u.Username == _username && u.Password == hashedPass);

            var authUser = validniKorisnik.FirstOrDefault();

            if (authUser != null)
            {
                App.ViewModel.AutorizovaniKorisnik = new UposlenikViewModel(authUser);
            }
                            
        }

    }
}
