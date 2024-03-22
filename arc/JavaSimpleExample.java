package model;

/*
 * Copyright (c) 2017 ObjectLabs Corporation
 * Distributed under the MIT license - http://opensource.org/licenses/MIT
 *
 * Written with mongo-3.4.2.jar
 * Documentation: http://api.mongodb.org/java/
 * A Java class connecting to a MongoDB database given a MongoDB Connection URI.
 */
import com.mongodb.MongoClient;
import com.mongodb.MongoClientURI;

import com.mongodb.client.MongoDatabase;
import com.mongodb.client.MongoCollection;

import org.bson.Document;

import com.mongodb.client.MongoCursor;
import java.util.ArrayList;
import java.util.List;

public class JavaSimpleExample {

    public static void main(String[] args) {

        // Create seed data
        List<Document> seedData = new ArrayList<Document>();

        seedData.add(new Document("decade", "1970s")
                .append("artist", "Debby Boone")
                .append("song", "You Light Up My Life")
                .append("weeksAtOne", 10)
        );

        seedData.add(new Document("decade", "1980s")
                .append("artist", "Olivia Newton-John")
                .append("song", "Physical")
                .append("weeksAtOne", 10)
        );

        seedData.add(new Document("decade", "1990s")
                .append("artist", "Mariah Carey")
                .append("song", "One Sweet Day")
                .append("weeksAtOne", 16)
        );

        // Standard URI format: mongodb://[dbuser:dbpassword@]host:port/dbname
        //mongodb://<dbuser>:<dbpassword>@ds039020.mlab.com:39020/mongolab-amazon-vibanda
        MongoClientURI uri = new MongoClientURI("mongodb://vibanda_mvp:iV72pq7Y!R@ds039020.mlab.com:39020/mongolab-amazon-vibanda");
        MongoClient client = new MongoClient(uri);

//        client.
        MongoDatabase db = client.getDatabase(uri.getDatabase());

        /*
         * First we'll add a few songs. Nothing is required to create the
         * songs collection; it is created automatically when we insert.
         */
        MongoCollection<Document> songs = db.getCollection("vibandalisting");

        // Note that the insert method can take either an array or a document.
        try {
            songs.insertMany(seedData);
        } catch (Exception e) {
            System.err.println("ERROR: " + e.getLocalizedMessage());
        }

        /*
         * Then we need to give Boyz II Men credit for their contribution to
         * the hit "One Sweet Day".
         */
        Document updateQuery = new Document("song", "One Sweet Day");
        songs.updateOne(updateQuery, new Document("$set", new Document("artist", "Mariah Carey ft. Boyz II Men")));

        /*
         * Finally we run a query which returns all the hits that spent 10 
         * or more weeks at number 1.
         */
        Document findQuery = new Document("weeksAtOne", new Document("$gte", 10));
        Document orderBy = new Document("decade", 1);

        MongoCursor<Document> cursor = songs.find(findQuery).sort(orderBy).iterator();

        try {
            while (cursor.hasNext()) {
                Document doc = cursor.next();
                System.out.println(
                        "In the " + doc.get("decade") + ", " + doc.get("song")
                        + " by " + doc.get("artist") + " topped the charts for "
                        + doc.get("weeksAtOne") + " straight weeks."
                );
            }
        } finally {
            cursor.close();
        }

        // Since this is an example, we'll clean up after ourselves.
//        songs.drop();

        // Only close the connection when your app is terminating
        client.close();
    }
}
