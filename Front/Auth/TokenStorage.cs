using Blazored.LocalStorage;

namespace Front.Auth
{
    public class TokenStorage
    {
        private const string Key = "auth_token";
        private readonly ILocalStorageService _localStorage;

        public TokenStorage(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public async Task SetTokenAsync(string token)
        {
            token = (token ?? "").Trim();
            await _localStorage.SetItemAsync(Key, token);
        }

        public async Task<string?> GetTokenAsync()
        {
            // مهم: GetItemAsync<string> علشان يجيب String خام مش JSON object
            var token = await _localStorage.GetItemAsync<string>(Key);

            // تنظيف قوي (حل IDX14102 غالباً)
            token = token?.Trim();
            token = token?.Trim('"');
            token = token?.Replace("\r", "").Replace("\n", "");
            Console.WriteLine("Token dot count = " + token?.Count(c => c == '.'));

            return token;
        }


        public async Task RemoveTokenAsync()
        {
            await _localStorage.RemoveItemAsync(Key);
        }
    }
}
