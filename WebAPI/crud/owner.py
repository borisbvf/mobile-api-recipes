from sqlalchemy.orm import Session
from schemas.owner import OwnerUpdateCode
from datetime import datetime

from models.owner import OwnerDB


def get_owner(db: Session, email: str) -> OwnerDB | None:
    return db.query(OwnerDB).filter(OwnerDB.email == email).first()

def create_owner(db: Session, email: str) -> OwnerDB:
    obj_db = OwnerDB(email=email)
    db.add(obj_db)
    db.commit()
    db.refresh(obj_db)
    return obj_db
    
def update_owner_code(db: Session, owner: OwnerUpdateCode) -> OwnerDB:
    obj_db = get_owner(db, owner.email)
    if obj_db is None:
        raise Exception
    obj_db.code = owner.code
    obj_db.code_updated = datetime.now()
    db.add(obj_db)
    db.commit()
    db.refresh(obj_db)
    return obj_db
    