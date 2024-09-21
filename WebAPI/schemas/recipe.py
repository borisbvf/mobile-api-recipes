from pydantic import BaseModel, Field

class RecipeBase(BaseModel):
    name: str = Field(title="Name of the recipe.", max_length=100)
    description: str | None = Field(title="Recipe's description", default=None, max_length=250)
    content: str = Field(title="The text of the recipe.", max_length=2000)

class RecipeFull(RecipeBase):
    #Tags: list[Tag]
    ingredients: list | None

class RecipeCreate(RecipeBase):
    pass
    #owner_id: int = Field(title="Owner id.")

class RecipeUpdate(RecipeBase):
    pass
    #id: int = Field(title="Recipe id")

class RecipeInDb(RecipeUpdate):
    pass