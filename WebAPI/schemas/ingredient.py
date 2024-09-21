from pydantic import BaseModel, Field

class Ingredient(BaseModel):
    name: str = Field(title="Name of the ingredient", max_length=200)
