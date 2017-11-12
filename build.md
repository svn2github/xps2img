# Prerequisites #

* Windows Vista/7/10
* Visual Studio 2010/2012/2013/2015/[2017](http://www.visualstudio.com/thank-you-downloading-visual-studio/?sku=Community)
* [Microsoft .NET Framework 4](http://www.microsoft.com/download/en/details.aspx?id=17851)
* [HTML Help Workshop](http://www.microsoft.com/download/en/details.aspx?id=21138)
* [Inno Setup](http://www.jrsoftware.org/isdl.php) 5.5.7 or higher.

# Process #

| Command         | Action         |
| --------------- | -------------- |
| build.bat       | Builds Release |
| build.bat debug | Builds Debug   |

# Output #

Can be found under solution dir ```_bin``` folder.


# Troubleshooting #

## Error MSB3091: Task failed because "resgen.exe" was not found ##

To fix apply one of registry files below after verifying SDK path.

| Registry File | Platform |
| --------------| -------- |
| build.x64.reg | x64      |
| build.x86.reg | x86      |

Details can be found [here](https://developercommunity.visualstudio.com/content/problem/18560/can-not-find-resgenexe-in-task.html).
