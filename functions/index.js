var Result = function(){
    return {
        success: false,
        response: null,
        error: null
    }
}

var Error = function(code, msg){
    return {
        code: code,
        msg: msg
    }
}

const admin = require('firebase-admin')
const functions = require('firebase-functions')
admin.initializeApp(functions.config().firebase)
const express = require('express')
const app = express()
let db = admin.firestore()
let tokensRef = db.collection("tokens")

// // Create and Deploy Your First Cloud Functions
// // https://firebase.google.com/docs/functions/write-firebase-functions
//
 exports.helloWorld = functions.https.onRequest((request, response) => {
  response.send("Hello from Firebase!");
});

app.get('/', async(req, res) => {
    let result = Result()
    const id = req.query.id
    if(id !== undefined){
        tokensRef.doc(id).get().then(data => {
            result.success = true
            result.response = data.data()
            res.send(result)
            return
        }).catch(error => {
            result.error = error
            res.send(result)
            return
        })
    }else{
        res.send(result)
    }
})

app.post('/create', async(req, res) => {
    let result = Result()
    let doc = {
        isShutdown: false,
        lastShutdown: new Date()
    }
    tokensRef.add(doc).then(success => {
        const id = success.id
        doc.id = id
        result.success = true
        result.response = doc
        res.send(result)
        return
    }).catch(error => {
        result.error = error
        res.send(result)
        return
    })
})

app.put('/unshutdown', async(req, res) => {
    let result = Result()
    const data = {
        isShutdown: false,
        lastShutdown: new Date()
    }
    if(req.query.id !== undefined){
        const id = req.query.id
        tokensRef.doc(id).set(data).then(() => {
            result.success = true
            res.send(result)
            return
        }).catch(error => {
            res.send(result)
            return
        })
    }else{
        res.send(result)
    }
})

app.get('/shutdown', async(req, res) => {
    let result = Result()
    const data = {
        isShutdown: true,
        lastShutdown: new Date()
    }
    if(req.query.id !== undefined){
        const id = req.query.id
        tokensRef.doc(id).set(data).then(() => {
            result.success = true
            res.send(result)
            return
        }).catch(error => {
            res.send(result)
            return
        })
    }else{
        res.send(result)
    }
})


exports.api = functions.https.onRequest(app);
