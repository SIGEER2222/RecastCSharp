// Thank for https://github.com/BlackWhiteYoshi/TSRuntime
#pragma warning disable
#nullable enable annotations

using Microsoft.JSInterop.Infrastructure;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Microsoft.JSInterop;

/// <summary>
/// <para>Interface for JS-interop.</para>
/// <para>It contains an invoke-method for every js-function, a preload-method for every module and a method to load all modules.</para>
/// </summary>
public interface ITSRuntime
{
  IJSRuntime JsRuntime { get; }
  Task<IJSObjectReference>?[] Modules { get; }
  Task<IJSObjectReference> GetOrLoadModule(int index, string url);

  /// <summary>
  /// <para>Fetches all modules as javascript-modules.</para>
  /// <para>If already loading, it doesn't trigger a second loading and if any already loaded, these are not loaded again, so if all already loaded, it returns a completed task.</para>
  /// </summary>
  /// <returns>A Task that will complete when all module loading Tasks have completed.</returns>
  Task PreloadAllModules()
  {
    return Task.WhenAll(Modules);
  }

  #region Example

  /// <summary>
  /// <para>Fetches 'Example' (/Example.razor.js) as javascript-module.</para>
  /// <para>If already loading, it doesn't trigger a second loading and if already loaded, it returns a completed task.</para>
  /// </summary>
  /// <returns>A Task that will complete when the module import have completed.</returns>
  Task PreloadExample() => GetOrLoadModule(0, "/Example.razor.js");

  /// <summary>
  /// Invokes in module 'Example' the JS-function 'example' synchronously when supported, otherwise asynchronously.
  /// </summary>
  /// <param name="cancellationToken">A cancellation token to signal the cancellation of the operation. Specifying this parameter will override any default cancellations such as due to timeouts (<see cref="JSRuntime.DefaultAsyncTimeout"/>) from being applied.</param>
  /// <returns>A Task that will complete when the JS-Function have completed.</returns>
  Task Example(CancellationToken cancellationToken = default)
  {
    ValueTask<IJSVoidResult> task = InvokeTrySync<IJSVoidResult>(0, "/Example.razor.js", "example", cancellationToken);
    return task.IsCompleted ? Task.CompletedTask : task.AsTask();
  }
  #endregion

  /// <summary>
  /// <para>Invokes the specified JavaScript function in the specified module synchronously.</para>
  /// <para>If module is not loaded, it returns without any invoking. If synchronous is not supported, it fails with an exception.</para>
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="moduleUrl">complete path of the module, e.g. "/Pages/Example.razor.js"</param>
  /// <param name="identifier">name of the javascript function</param>
  /// <param name="success">false when the module is not loaded, otherwise true</param>
  /// <param name="args">parameter passing to the JS-function</param>
  /// <returns>default when the module is not loaded, otherwise result of the JS-function</returns>
  private TResult Invoke<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)] TResult>(int moduleIndex, string moduleUrl, string identifier, params object?[]? args)
  {
    Task<IJSObjectReference> moduleTask = GetOrLoadModule(moduleIndex, moduleUrl);
    if (!moduleTask.IsCompletedSuccessfully)
      throw new JSException("JS-module is not loaded. Use and await the Preload-method to ensure the module is loaded.");

    return ((IJSInProcessObjectReference)moduleTask.Result).Invoke<TResult>(identifier, args);
  }

  /// <summary>
  /// Invokes the specified JavaScript function in the specified module synchronously when supported, otherwise asynchronously.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="moduleUrl">complete path of the module, e.g. "/Pages/Example.razor.js"</param>
  /// <param name="identifier">name of the javascript function</param>
  /// <param name="cancellationToken">A cancellation token to signal the cancellation of the operation. Specifying this parameter will override any default cancellations such as due to timeouts (<see cref="JSRuntime.DefaultAsyncTimeout"/>) from being applied.</param>
  /// <param name="args">parameter passing to the JS-function</param>
  /// <returns></returns>
  private async ValueTask<TValue> InvokeTrySync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)] TValue>(int moduleIndex, string moduleUrl, string identifier, CancellationToken cancellationToken, params object?[]? args)
  {
    IJSObjectReference module = await GetOrLoadModule(moduleIndex, moduleUrl);
    if (module is IJSInProcessObjectReference inProcessModule)
      return inProcessModule.Invoke<TValue>(identifier, args);
    else
      return await module.InvokeAsync<TValue>(identifier, cancellationToken, args);
  }

  /// <summary>
  /// Invokes the specified JavaScript function in the specified module asynchronously.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="moduleUrl">complete path of the module, e.g. "/Pages/Example.razor.js"</param>
  /// <param name="identifier">name of the javascript function</param>
  /// <param name="cancellationToken">A cancellation token to signal the cancellation of the operation. Specifying this parameter will override any default cancellations such as due to timeouts (<see cref="JSRuntime.DefaultAsyncTimeout"/>) from being applied.</param>
  /// <param name="args">parameter passing to the JS-function</param>
  /// <returns></returns>
  private async ValueTask<TValue> InvokeAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)] TValue>(int moduleIndex, string moduleUrl, string identifier, CancellationToken cancellationToken, params object?[]? args)
  {
    IJSObjectReference module = await GetOrLoadModule(moduleIndex, moduleUrl);
    return await module.InvokeAsync<TValue>(identifier, cancellationToken, args);
  }


  #region JSRuntime methods
  #endregion
}

public static class TSRuntimeServiceExctension
{
  /// <summary>
  /// Registers a scoped ITSRuntime with a TSRuntime as implementation and if available, registers the module interfaces with the same TSRuntime-object.
  /// </summary>
  /// <param name="services"></param>
  /// <returns></returns>
  public static IServiceCollection AddTSRuntime(this IServiceCollection services)
  {
    services.AddScoped<ITSRuntime, TSRuntime>();

    return services;
  }
}
