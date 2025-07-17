using ToolNexus.Application.DTOs;
using ToolNexus.Application.Interfaces;
using ToolNexus.Domain.ToolCategories;

namespace ToolNexus.Application.Services;

public class ToolCategoryService : IToolCategoryService
{
    private readonly IToolCategoryRepository _repository;

    public ToolCategoryService(IToolCategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<ToolCategoryDto?> GetByIdAsync(int id)
    {
        var category = await _repository.GetByIdAsync(id);
        if (category == null) return null;

        return MapToDto(category);
    }

    public async Task<IEnumerable<ToolCategoryDto>> GetAllAsync()
    {
        var categories = await _repository.GetAllAsync();
        return categories.Select(MapToDto);
    }

    public async Task<IEnumerable<ToolCategoryDto>> GetActiveAsync()
    {
        var categories = await _repository.GetActiveAsync();
        return categories.Select(MapToDto);
    }

    public async Task<ToolCategoryDto> CreateAsync(CreateToolCategoryDto dto)
    {
        if (await _repository.CodeExistsAsync(dto.Code))
        {
            throw new InvalidOperationException($"Kategorija s kodo '{dto.Code}' že obstaja.");
        }

        var category = new ToolCategory
        {
            Code = dto.Code,
            Name = dto.Name,
            Description = dto.Description,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        var id = await _repository.AddAsync(category);
        category.Id = id;

        return MapToDto(category);
    }

    public async Task<ToolCategoryDto> UpdateAsync(UpdateToolCategoryDto dto)
    {
        var category = await _repository.GetByIdAsync(dto.Id);
        if (category == null)
        {
            throw new InvalidOperationException($"Kategorija z ID {dto.Id} ne obstaja.");
        }

        if (await _repository.CodeExistsAsync(dto.Code, dto.Id))
        {
            throw new InvalidOperationException($"Kategorija s kodo '{dto.Code}' že obstaja.");
        }

        category.Code = dto.Code;
        category.Name = dto.Name;
        category.Description = dto.Description;
        category.IsActive = dto.IsActive;
        category.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(category);

        return MapToDto(category);
    }

    public async Task DeleteAsync(int id)
    {
        var category = await _repository.GetByIdAsync(id);
        if (category == null)
        {
            throw new InvalidOperationException($"Kategorija z ID {id} ne obstaja.");
        }

        if (category.Tools.Any())
        {
            throw new InvalidOperationException("Kategorije ni mogoče izbrisati, ker vsebuje orodja.");
        }

        await _repository.DeleteAsync(id);
    }

    public async Task<bool> CodeExistsAsync(string code, int? excludeId = null)
    {
        return await _repository.CodeExistsAsync(code, excludeId);
    }

    private static ToolCategoryDto MapToDto(ToolCategory category)
    {
        return new ToolCategoryDto
        {
            Id = category.Id,
            Code = category.Code,
            Name = category.Name,
            Description = category.Description,
            IsActive = category.IsActive,
            CreatedAt = category.CreatedAt,
            CreatedBy = category.CreatedBy,
            UpdatedAt = category.UpdatedAt,
            UpdatedBy = category.UpdatedBy,
            ToolCount = category.Tools?.Count ?? 0
        };
    }
}