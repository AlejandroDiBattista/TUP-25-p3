using Microsoft.JSInterop;

namespace cliente.Services;

public class CartStorage
{
    private readonly IJSRuntime _js;

    public CartStorage(IJSRuntime js)
    {
        _js = js;
    }

    public async Task<string?> GetCartIdAsync()
    {
        return await _js.InvokeAsync<string>("cartStorage.get");
    }

    public async Task SetCartIdAsync(string id)
    {
        await _js.InvokeVoidAsync("cartStorage.set", id);
    }
}
