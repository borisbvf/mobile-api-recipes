from datetime import timedelta
from fastapi import APIRouter, Query, HTTPException, status, Depends, Response
from fastapi.security import HTTPBasic, HTTPBasicCredentials
from typing import Annotated
from sqlalchemy.orm import Session

from crud.owner import get_owner, create_owner, update_owner_code
from schemas.owner import OwnerUpdateCode
import auth
import mail_utils
from dependencies import get_local_session

owner_router = APIRouter()

security = HTTPBasic()

@owner_router.post("/code_query")
async def send_code(
        email: Annotated[str, Query(title="Email to register new user.")],
        db: Session = Depends(get_local_session)
        ):
    email = "".join(letter for letter in email if letter not in ['"', "'"])
    owner = get_owner(db, email)
    if not owner:
        owner = create_owner(db, email)
    new_code = auth.generate_code()
    owner_update = OwnerUpdateCode(email=email, code=new_code)
    update_owner_code(db, owner_update)
    mail_utils.send_code(email, new_code)
    return Response(status_code=status.HTTP_200_OK)
    
@owner_router.get("/token")
async def create_token(
        credentials: Annotated[HTTPBasicCredentials, Depends(security)],
        db: Session = Depends(get_local_session)
        ) -> auth.Token:
    username = credentials.username
    password = credentials.password
    owner = get_owner(db, username)
    if not owner:
        raise HTTPException(
            status_code=status.HTTP_401_UNAUTHORIZED,
            detail="Incorrect email or code",
            headers={"WWW-Authenticate": "Bearer"},
        )
    hashed_code = auth.pwd_context.hash(owner.code)
    if not auth.pwd_context.verify(password, hashed_code):
        raise HTTPException(
            status_code=status.HTTP_401_UNAUTHORIZED,
            detail="Incorrect email or code",
            headers={"WWW-Authenticate": "Bearer"},
        )
    access_token_expires = timedelta(minutes=auth.ACCESS_TOKEN_EXPIRE_MINUTES)
    access_token = auth.create_access_token(data={"sub": owner.email}, expires_delta=access_token_expires)
    return auth.Token(access_token=access_token, token_type="bearer")