import pickle

embedding_dictionary = {}

with open("embedding_dictionary.pkl", "wb") as file:
    pickle.dump(embedding_dictionary, file)
    