conn = new Mongo();
db = conn.getDB("demo");

const documents = [
    { _id: '052d36c3-6a92-4217-9f7e-986bc6338e59', Name: 'Motherboard', Quantity: 23 },
    { _id: '3208ca85-e75e-401f-9f4a-0e99dee2bb96', Name: 'Keyboard', Quantity: 4 },
    { _id: '876ac1ba-c128-4cf2-a25b-dfae44ae44e2', Name: 'Mouse', Quantity: 7 },
];

db.Product.insertMany(documents);
