namespace ProductManager.Persistence.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    public CategoryRepository(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<ApiResponse> GetCategoriesAsync()
    {
        var response = await _context.Categories.ToListAsync();
        var categories = _mapper.Map<List<GetCategoryDto>>(response);
        return new ApiResponse(200, "", categories);
    }
    public async Task<ApiResponse> GetCategoryAsync(int id)
    {
        var response = await _context.Categories.FirstOrDefaultAsync(x => x.CategoryId == id);
        if (response == null)
        {
            return new ApiResponse(404, "Category not found");
        }
        var category = _mapper.Map<GetCategoryDto>(response);
        return new ApiResponse(200, "", category);
    }
    public async Task<ApiResponse> CreateCategoryAsync(CreateCategoryDto createCategoryDto)
    {
        try
        {
            var checkCategory = await _context.Categories.FirstOrDefaultAsync(x => x.CategoryName == createCategoryDto.CategoryName);
            if (checkCategory != null)
            {
                return new ApiResponse(404, "Category already exists");
            }
            var category = _mapper.Map<Category>(createCategoryDto);
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return new ApiResponse(200, "Category created successfully");
        }
        catch (Exception e)
        {
            return new ApiResponse(500, e.Message);
        }
    }
}
