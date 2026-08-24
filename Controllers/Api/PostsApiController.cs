using BlogManager.Dtos;
using BlogManager.Models.Entities;
using BlogManager.Security;
using BlogManager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogManager.Controllers.Api;

[ApiController]
[Route("api/posts")]
public class PostsApiController(
    IPostService postService,
    ICategoryService categoryService,
    ITagService tagService) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType<PagedResponseDto<PostDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponseDto<PostDto>>> GetAll(
        string? search,
        string? sort,
        int? tagId,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var safePageSize = Math.Clamp(pageSize, 1, 100);
        var normalizedSort = sort is "title" or "oldest" or "popular" ? sort : null;
        var result = await postService.GetPageAsync(
            search?.Trim(),
            normalizedSort,
            tagId,
            page,
            safePageSize,
            cancellationToken);

        return Ok(new PagedResponseDto<PostDto>
        {
            Items = result.Posts.Select(MapToDto).ToList(),
            Page = result.PageNumber,
            PageSize = safePageSize,
            TotalItems = result.TotalCount,
            TotalPages = result.TotalPages
        });
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType<PostDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PostDto>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var post = await postService.GetByIdAsync(id, cancellationToken);
        return post is null ? NotFound() : Ok(MapToDto(post));
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType<PostDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PostDto>> Create(
        PostSaveDto dto,
        CancellationToken cancellationToken)
    {
        Normalize(dto);
        ModelState.Clear();
        TryValidateModel(dto);
        if (!await ValidateRelationsAsync(dto, cancellationToken))
        {
            return ValidationProblem(ModelState);
        }

        var post = MapToEntity(dto);
        await postService.CreateAsync(post, dto.TagIds, cancellationToken);
        var createdPost = await postService.GetByIdAsync(post.Id, cancellationToken);
        return CreatedAtAction(
            nameof(GetById),
            new { id = post.Id },
            MapToDto(createdPost!));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = RoleNames.CanEditPosts)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        int id,
        PostSaveDto dto,
        CancellationToken cancellationToken)
    {
        Normalize(dto);
        ModelState.Clear();
        TryValidateModel(dto);
        if (!await ValidateRelationsAsync(dto, cancellationToken))
        {
            return ValidationProblem(ModelState);
        }

        var post = MapToEntity(dto);
        post.Id = id;
        return await postService.UpdateAsync(post, dto.TagIds, cancellationToken)
            ? NoContent()
            : NotFound();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = RoleNames.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        return await postService.DeleteAsync(id, cancellationToken)
            ? NoContent()
            : NotFound();
    }

    private async Task<bool> ValidateRelationsAsync(
        PostSaveDto dto,
        CancellationToken cancellationToken)
    {
        if (dto.CategoryId.HasValue &&
            !await categoryService.ExistsAsync(dto.CategoryId.Value, cancellationToken))
        {
            ModelState.AddModelError(nameof(dto.CategoryId), "Danh mục đã chọn không tồn tại.");
        }

        if (!await tagService.AllExistAsync(dto.TagIds, cancellationToken))
        {
            ModelState.AddModelError(nameof(dto.TagIds), "Một hoặc nhiều thẻ đã chọn không tồn tại.");
        }

        return ModelState.IsValid;
    }

    private static void Normalize(PostSaveDto dto)
    {
        dto.Title = dto.Title?.Trim() ?? string.Empty;
        dto.Content = dto.Content?.Trim() ?? string.Empty;
        dto.Author = dto.Author?.Trim() ?? string.Empty;
        dto.TagIds = dto.TagIds.Distinct().ToList();
    }

    private static Post MapToEntity(PostSaveDto dto)
    {
        return new Post
        {
            Title = dto.Title,
            Content = dto.Content,
            Author = dto.Author,
            PublishedAt = dto.PublishedAt,
            IsPublished = dto.IsPublished,
            CategoryId = dto.CategoryId
        };
    }

    private static PostDto MapToDto(Post post)
    {
        return new PostDto
        {
            Id = post.Id,
            Title = post.Title,
            Content = post.Content,
            Author = post.Author,
            PublishedAt = post.PublishedAt,
            IsPublished = post.IsPublished,
            ViewCount = post.ViewCount,
            CategoryId = post.CategoryId,
            CategoryName = post.Category?.Name,
            Tags = post.Tags
                .OrderBy(tag => tag.Name)
                .Select(tag => new TagDto(tag.Id, tag.Name))
                .ToList()
        };
    }
}
