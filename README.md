[logo]: https://raw.githubusercontent.com/Geeksltd/Zebble.SearchInput/master/Shared/NuGet/Icon.png "Zebble.SearchInput"


## Zebble.SearchInput

![logo]

A Zebble plugin that allow you to make searchable list or page.


[![NuGet](https://img.shields.io/nuget/v/Zebble.SearchInput.svg?label=NuGet)](https://www.nuget.org/packages/Zebble.SearchInput/)

> Search Input is a customised TextInput that allows you to call a method (for example : Reload) after text completing method. You can set the Image and Cancel Button text and Placeholder for that.

<br>


### Setup
* Available on NuGet: [https://www.nuget.org/packages/Zebble.SearchInput/](https://www.nuget.org/packages/Zebble.SearchInput/)
* Install in your platform client projects.
* Available for iOS, Android and UWP.
<br>


### Api Usage

To use this plugin in Zebble application you can use below code:
```xml
<SearchInput Id="MySearchInput" on-Searched="ApplySearch" />
```

You can handle the Searched event to apply your search logic. Usually you will first extract the keywords from the SearchInput and then use that in your search implementation logic.
```csharp
Product[] Items;

async Task ApplySearch()
{
     var keywords = MySearchInput.Text.Split(' ').Trim().ToArray();

     // In this example I want to pass the keywords to an API to implement the search logic on the server.
     Items = await Api.SearchProducts(keywords);

     // Now you can use the data any how you like. For example:
     await MyListView.UpdateSource(Items);     
}
```
### Properties
| Property     | Type         | Android | iOS | Windows |
| :----------- | :----------- | :------ | :-- | :------ |
| Text           | String           | x       | x   | x       |

### Events
| Event             | Type                                          | Android | iOS | Windows |
| :-----------      | :-----------                                  | :------ | :-- | :------ |
| Searched              | AsyncEvent    | x       | x   | x       |
| Searching              | AsyncEvent    | x       | x   | x       |

### Methods
| Method       | Return Type  | Parameters                          | Android | iOS | Windows |
| :----------- | :----------- | :-----------                        | :------ | :-- | :------ |
| Focus         | Void| -| x       | x   | x       |
| UnFocus         | Void| -| x       | x   | x       |
