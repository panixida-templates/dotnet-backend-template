//using Application.Abstractions.Queries;
//using Application.Users.Search;

//using Riok.Mapperly.Abstractions;

//namespace Presentation.Http.Features.Users.Search;

//[Mapper]
//internal static partial class SearchUsersMapper
//{
//    internal static partial SearchUserItemResponse ToItemResponse(SearchUserDto dto);

//    internal static SearchUsersResponse ToResponse(SearchResult<SearchUserDto> result)
//    {
//        var items = result.Items
//            .Select(ToItemResponse)
//            .ToArray();

//        return new SearchUsersResponse(items, result.TotalCount);
//    }
//}
