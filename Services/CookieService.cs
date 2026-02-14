using Microsoft.JSInterop;

namespace CMetalsFulfillment.Services
{
    public class CookieService(IJSRuntime jsRuntime) : IAsyncDisposable
    {
        private IJSObjectReference? _module;

        private async Task<IJSObjectReference> GetModuleAsync()
        {
            return _module ??= await jsRuntime.InvokeAsync<IJSObjectReference>("import", "./js/cookie.js");
        }

        public async Task SetCookieAsync(string name, string value, int days)
        {
            var module = await GetModuleAsync();
            await module.InvokeVoidAsync("setCookie", name, value, days);
        }

        public async Task<string?> GetCookieAsync(string name)
        {
            var module = await GetModuleAsync();
            return await module.InvokeAsync<string?>("getCookie", name);
        }

        public async ValueTask DisposeAsync()
        {
            if (_module != null)
            {
                await _module.DisposeAsync();
            }
        }
    }
}
