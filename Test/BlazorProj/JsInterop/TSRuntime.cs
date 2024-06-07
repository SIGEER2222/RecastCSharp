// Thank for https://github.com/BlackWhiteYoshi/TSRuntime
#pragma warning disable
#nullable enable annotations

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.JSInterop;

/// <summary>
/// <para>An implementation for <see cref="ITSRuntime"/>.</para>
/// <para>It manages JS-modules: It loads the modules, caches it in an array and disposing releases all modules.</para>
/// </summary>
public sealed class TSRuntime : ITSRuntime, IDisposable, IAsyncDisposable
{
  #region construction
  private readonly IJSRuntime _jsRuntime;
  public IJSRuntime JsRuntime => _jsRuntime;

  public TSRuntime(IJSRuntime jsRuntime)
  {
    _jsRuntime = jsRuntime;
    modules = new Task<IJSObjectReference>?[BlazorProj.Generator.GeneratorHelp.TSRuntimeCount];
  }

  #endregion

  #region disposing

  private readonly CancellationTokenSource cancellationTokenSource = new();

  /// <summary>
  /// Releases each module synchronously if possible, otherwise asynchronously per fire and forget.
  /// </summary>
  public void Dispose()
  {
    if (cancellationTokenSource.IsCancellationRequested)
      return;

    cancellationTokenSource.Cancel();
    cancellationTokenSource.Dispose();

    for (int i = 0; i < modules.Length; i++)
    {
      Task<IJSObjectReference>? module = modules[i];

      if (module?.IsCompletedSuccessfully == true)
        if (module.Result is IJSInProcessObjectReference inProcessModule)
          inProcessModule.Dispose();
        else
          _ = module.Result.DisposeAsync().Preserve();

      modules[i] = null;
    }
  }

  /// <summary>
  /// <para>Releases each module synchronously if possible, otherwise asynchronously and returns a task that completes, when all module disposing tasks complete.</para>
  /// <para>The asynchronous disposing tasks are happening in parallel.</para>
  /// </summary>
  /// <returns></returns>
  public ValueTask DisposeAsync()
  {
    if (cancellationTokenSource.IsCancellationRequested)
      return ValueTask.CompletedTask;

    cancellationTokenSource.Cancel();
    cancellationTokenSource.Dispose();

    List<Task> taskList = new(modules.Length);
    for (int i = 0; i < modules.Length; i++)
    {
      Task<IJSObjectReference>? module = modules[i];

      if (module?.IsCompletedSuccessfully == true)
        if (module.Result is IJSInProcessObjectReference inProcessModule)
          inProcessModule.Dispose();
        else
        {
          ValueTask valueTask = module.Result.DisposeAsync();
          if (!valueTask.IsCompleted)
            taskList.Add(valueTask.AsTask());
        }

      modules[i] = null;
    }

    if (taskList.Count == 0)
      return ValueTask.CompletedTask;
    else
      return new ValueTask(Task.WhenAll(taskList));
  }

  #endregion

  #region moduleList

  private readonly Task<IJSObjectReference>?[] modules;
  public Task<IJSObjectReference>?[] Modules => modules;

  /// <summary>
  /// <para>Loads and caches the module loading Tasks in an array.</para>
  /// <para>The first time it creates the module loading Task with the given url and stores it at the given index, all subsequent calls return the stored Task.</para>
  /// </summary>
  /// <param name="index">index of the array element.</param>
  /// <param name="url">URL to fetch if module is not loaded yet.</param>
  /// <returns></returns>
  public Task<IJSObjectReference> GetOrLoadModule(int index, string url)
  {
    if (modules[index]?.IsCompletedSuccessfully == true)
      return modules[index]!;
    return modules[index] = _jsRuntime.InvokeAsync<IJSObjectReference>("import", cancellationTokenSource.Token, url).AsTask();
  }
  #endregion
}