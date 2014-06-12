pvc.Task("nuget-push", () => {
    pvc.Source("src/Pvc.Xunit.csproj")
       .Pipe(new PvcNuGetPack(
            createSymbolsPackage: true
       ))
       .Pipe(new PvcNuGetPush());
});
