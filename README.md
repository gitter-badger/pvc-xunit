pvc-xunit
===========
[![Gitter](https://badges.gitter.im/Join Chat.svg)](https://gitter.im/kwonoj/pvc-xunit?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

###What

[PVC Build Engine](http://pvcbuild.com) plugin to execute testcases written for [xUnit test framework](https://github.com/xunit/xunit).

###Sample

To run test cases once, you'd create pvc task as below :

*pvcfile.csx*
```
pvc.Task("TestRunner", ()=>{
	pvc.Source("*.Test.dll")
	.Pipe(new PvcXunit());
});
```

You can monitor code changes continously as below (requires [pvc-msbuild](https://github.com/pvcbuild/pvc-msbuild)) :

*pvcfile.csx*
```
pvc.Task("TestRunner", ()=>{
	pvc.Source("*.Test.dll")
	.Pipe(new PvcXunit())
	.Watch();
});

pvc.Task("BuildMonitor", ()=>{
	pvc.Source("...\\Solution.sln")
	.Ignore(ArtifactGlobs.VisualStudio)
	.Pipe(new PvcMSBuild())
	.Watch("**/*.cs");
});

//Note : this'll run whole testcases everytime build is triggered!
pvc.Task("TestMonitor", ()=>{}).Requires("TestRunner","BuildMonitor");
```

Output verbose option can be controlled : 

*pvcfile.csx*
```
pvc.Task("TestRunner", ()=>{
	pvc.Source("*.Test.dll")
	.Pipe(new PvcXunit(displaySuccess: true, displayFailureStack: true));
})
```

### Note

Due to implementation of xUnit v1 test runner, xunit.dll should be placed where testcase assemblies are located.