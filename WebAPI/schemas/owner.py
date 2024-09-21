from pydantic import BaseModel, Field

class OwnerBase(BaseModel):
    email: str = Field(title="User's email address.", max_length=50)

class OwnerUpdateCode(OwnerBase):
    code: str = Field(title="Code for authentication.", max_length=20)

class Owner(OwnerBase):
    id: int = Field(title="User's id.")
