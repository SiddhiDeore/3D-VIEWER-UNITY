from flask import Flask, request, jsonify
from urllib.parse import unquote
import json
import bcrypt
import sqlalchemy as db
from sqlalchemy.orm import sessionmaker, scoped_session
from models import GameObject, Base, User  # Ensure User is defined in models.py

app = Flask(__name__)

# Database configuration
DATABASE_URI = 'mysql+mysqlconnector://root:nilukirti26%40@localhost/gameobjects'

engine = db.create_engine(DATABASE_URI)
Session = scoped_session(sessionmaker(bind=engine))  # Use scoped_session for thread safety
Base.metadata.create_all(engine)  # Ensure all tables are created

# Route to receive JSON data from Unity and update the database
@app.route('/update_data', methods=['POST'])
def update_data():
    if not request.is_json:
        return jsonify({'message': 'Invalid content type'}), 415
    data = request.get_json()
    for name, properties in data.items():
        properties_json = json.dumps(properties)
        game_object = Session.query(GameObject).filter_by(name=name).first()
        if game_object:
            game_object.data = properties_json
        else:
            game_object = GameObject(name=name, data=properties_json)
            Session.add(game_object)
    Session.commit()
    return jsonify({'message': 'Data updated successfully'})

# Route to retrieve data from the database and send it to Unity
@app.route('/get_data', methods=['GET'])
def get_data():
    data_dict = {"modelProperties": {}}
    game_objects = Session.query(GameObject).all()
    for game_object in game_objects:
        data_dict["modelProperties"][game_object.name] = json.loads(game_object.data)
    return jsonify(data_dict)

# Route for user login validation
@app.route('/validate-login', methods=['POST'])
def validate_login():
    if not request.is_json:
        return jsonify({'valid': False, 'message': 'Unsupported Media Type, please send JSON'}), 415
    data = request.get_json()
    username = data.get('username')
    password = data.get('password')
    if not username or not password:
        return jsonify({'valid': False, 'message': 'Missing username or password'})
    user = Session.query(User).filter_by(username=username).first()
    if user and bcrypt.checkpw(password.encode(), user.password.encode()):
        return jsonify({'valid': True, 'message': 'Login successful'})
    else:
        return jsonify({'valid': False, 'message': 'Invalid username or password'})

if __name__ == '__main__':
    app.run(debug=True)
