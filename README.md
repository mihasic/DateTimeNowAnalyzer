# DateTimeNowAnalyzer

![screen](https://cloud.githubusercontent.com/assets/266282/12216672/f7323c12-b6e9-11e5-887a-4482dbab49cc.gif)

Simple Analyzer for DateTime.Now, DateTime.UtcNow, DateTimeOffset.Now, DateTimeOffset.UtcNow.

Each of those static methods can lead to side-effects, that's why best practice is to have a top level abstraction for the project. This, for example, would allow to write unit tests with predictable results, without the need to deal with time accuracy.

DateTime.Now has way more issues than just being static mutable, but that's not really the goal of particular extension.
