## Introduction

This README identifies deviations and changes as compared to the source project. Please refer to [README.md](README.md) for MTAPI details.

This is a clone of [lazou/MTAPI](https://github.com/lazou/mtapi), being what appears to be the most recent active branch of [vdemydiuk/MTAPI](https://github.com/vdemydiuk/mtapi). The limitation being that this branch focuses on MT5 only and is not monitoring impact to MT4 build. Code changes appears to attempt to separate MT5 changes from the original.

A second, more active release from [VeriEasy/MTAPI]() seems to support both MT4/MT5 but appears to be in the process of major refactoring or something. So I am just going to keep an eye on it for now.

## Build environment
This branch is using Visual Studio 2022 Preview.

I am likely to include my MetaEditor build scripts into this project.

Use [MetaEditor](https://www.metatrader5.com/en/automated-trading/metaeditor) to compile MQL files (`*.mq5`) into `*.ex5`.

## TODO

* Evaluate this project for: MT5 to MT5, MT5 to MT4
* Check logging capability, incorporate [cshawky/CsTool.Logger](https://github.com/cshawky/CsTool.Logger) if needs be for a better diagnostic experience.

## Version History

