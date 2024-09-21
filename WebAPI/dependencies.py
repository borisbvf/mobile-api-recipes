from fastapi import Depends, status
from fastapi.security import OAuth2PasswordBearer
import jwt
from sqlalchemy.orm import Session

from database.session import session_maker
from auth import oauth2_scheme, TokenPayload, SECRET_KEY, ALGORITHM
from models.owner import OwnerDB
from crud.owner import get_owner

def get_local_session():
    db = session_maker()
    try:
        yield db
    finally:
        db.close()

def get_token(token: str = Depends(oauth2_scheme)) -> TokenPayload:
    try:
        payload = jwt.decode(token, SECRET_KEY, ALGORITHM)
        token_data = TokenPayload(**payload)
    except (jwt.PyJWTError) as e:
        raise Exception
    return token_data

def get_current_owner(
        db: Session = Depends(get_local_session), 
        token: TokenPayload = Depends(get_token)
        ) -> OwnerDB:
    owner = get_owner(db, token.sub)
    if owner is None:
        raise Exception
    return owner
