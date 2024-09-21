from sqlalchemy import create_engine
from sqlalchemy.orm import sessionmaker
import os

user = os.getenv('PG_USER', default='postgres')
pswd = os.getenv('PG_PSWD')
host = os.getenv('PG_HOST', default='localhost')
port = os.getenv('PG_PORT', default='5432')
pgdb = os.getenv('PG_DB', default='recipes')
conn_str = f"postgresql://{user}:{pswd}@{host}:{port}/{pgdb}"

db_engine = create_engine(conn_str)

session_maker = sessionmaker(autoflush=False, bind=db_engine)