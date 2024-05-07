import sqlalchemy as db
from sqlalchemy.orm import declarative_base
import bcrypt

Base = declarative_base()

class GameObject(Base):
    __tablename__ = 'game_objects'
    id = db.Column(db.Integer, primary_key=True)
    name = db.Column(db.String(255))
    data = db.Column(db.Text)

    def __repr__(self):
        return f"<GameObject(name={self.name}, data={self.data})>"

class User(Base):
    __tablename__ = 'users'
    id = db.Column(db.Integer, primary_key=True)
    username = db.Column(db.String(255), unique=True, nullable=False)
    password = db.Column(db.String(255), nullable=False)

    def __repr__(self):
        return f"<User(username={self.username})>"

    def set_password(self, password):
        self.password = bcrypt.hashpw(password.encode(), bcrypt.gensalt()).decode('utf-8')

    def check_password(self, password):
        return bcrypt.checkpw(password.encode(), self.password.encode())
