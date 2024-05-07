import json
import bcrypt
from sqlalchemy import create_engine, Column, Integer, String
from sqlalchemy.ext.declarative import declarative_base
from sqlalchemy.orm import sessionmaker
import os 

# Database configuration
DATABASE_URI = 'mysql+mysqlconnector://root:nilukirti26%40@localhost/gameobjects'
engine = create_engine(DATABASE_URI)
Session = sessionmaker(bind=engine)
session = Session()
Base = declarative_base()

class User(Base):
    __tablename__ = 'users'
    id = Column(Integer, primary_key=True)
    username = Column(String(255), unique=True, nullable=False)
    password = Column(String(255), nullable=False)

    def __repr__(self):
        return f"<User(username={self.username})>"

# Create the database table
Base.metadata.create_all(engine)

def load_and_insert_users(json_path):
    """Loads user data from a JSON file and inserts it into the database with hashed passwords."""
    with open(json_path, 'r') as file:
        data = json.load(file)['users']  # Load users from JSON file
        for user in data:
            # Hash the password
            hashed_password = bcrypt.hashpw(user['password'].encode(), bcrypt.gensalt())
            # Create a new User instance
            new_user = User(username=user['username'], password=hashed_password.decode('utf-8'))
            # Add the new user to the session
            session.add(new_user)
        # Commit the session to save the users to the database
        session.commit()

if __name__ == '__main__':
    json_file_path = os.path.join('..','..','Client','Assets','Resources','user.json')  # Adjust the path to your JSON file
    load_and_insert_users(json_file_path)
    print("Users loaded successfully into the database.")
