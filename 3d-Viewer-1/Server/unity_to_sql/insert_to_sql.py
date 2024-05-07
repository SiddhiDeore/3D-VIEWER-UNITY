import json
import sqlalchemy as db
from sqlalchemy.orm import declarative_base, sessionmaker
import os

# Database configuration
DATABASE_URI = 'mysql+mysqlconnector://root:nilukirti26%40@localhost/gameobjects'

engine = db.create_engine(DATABASE_URI)
Session = sessionmaker(bind=engine)
session = Session()
Base = declarative_base()

class GameObject(Base):
    __tablename__ = 'game_objects'
    id = db.Column(db.Integer, primary_key=True)
    name = db.Column(db.String(255))
    data = db.Column(db.Text)

    def __repr__(self):
        return f"<GameObject(name={self.name}, data={self.data})>"

# Create the table
Base.metadata.create_all(engine)

def load_and_insert_data(json_path):
    with open(json_path, 'r') as file:
        data = json.load(file)
        for name, properties in data.items():
            # Convert the properties dictionary to a JSON string
            properties_json = json.dumps(properties)

            # Create and add the new GameObject
            game_object = GameObject(name=name, data=properties_json)
            session.add(game_object)

        session.commit()

if __name__ == '__main__':
    json_file_path = os.path.join('..','..','Client','Assets','Resources','gameObjects1.json')  # Adjust the path to your JSON file
    load_and_insert_data(json_file_path)
