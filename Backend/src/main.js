/*
This is the entry point of the Backend application.
 */

import zmq from 'zeromq'
import Recorder from "./Recorder.js";
import CodeGenerator from "./CodeGenerator.js";
import Optimizer from "./Optimizer.js";
import {receiveMsg, sendAck, sendNak, sendMsg} from './ZeroMQUtils.js'

let sock = new zmq.Pair
sock.connect('tcp://127.0.0.1:3000')

const recorder = new Recorder();
recorder.on('onActionRecorded', (type, action, actions) =>
    sock.send(JSON.stringify(action)))

let browser, codeGen, actions, code, launched
launched = false

async function exitHandler() {
    if(launched)
        await recorder.close()

    sock.close()
}

process.on('exit', exitHandler)
process.on('SIGINT', exitHandler)
process.on('SIGUSR1', exitHandler)
process.on('SIGUSR2', exitHandler)


/*
Description: This method process commands from the Frontend application.
cmd: command to be processed
 */
function processCmd(cmd) {
    return new Promise(async (resolve, reject) => {
        try {
            switch (cmd) {
                case 'setEventsToRecord':
                    cmd = JSON.parse(await receiveMsg(sock))
                    recorder.setEventsToRecord(cmd)
                    break
                case 'launch':
                    cmd = await receiveMsg(sock) //launchOptions
                    try {
                        await recorder.launch(JSON.parse(cmd))
                        browser = recorder.getNativeBrowser()
                        launched = true
                        await sendAck(sock)
                    }
                    catch(ex) {
                        await sendNak(sock)
                    }
                    break
                case 'connect':
                    cmd = await receiveMsg(sock) //connectOptions
                    try {
                        await recorder.connect(JSON.parse(cmd))
                        browser = recorder.getNativeBrowser()
                        await sendAck(sock)
                    }
                    catch(ex) {
                        await sendNak(sock)
                    }
                    break
                case 'close':
                    launched = false
                    await recorder.close()
                    break
                case 'disconnect':
                    recorder.disconnect()
                    break
                case 'start':
                    await recorder.start(false)
                    break
                case 'startClean':
                    await recorder.start(true)
                    break
                case 'stop':
                    await recorder.stop()
                    break
                case 'optimize':
                    cmd = JSON.parse(await receiveMsg(sock))
                    const opt = new Optimizer()
                    cmd = opt.optimizeRecordings(cmd)
                    await sendMsg(sock, JSON.stringify(cmd))
                    break
                case 'codeGen':
                    cmd = await receiveMsg(sock) //codeGen options
                    actions = await receiveMsg(sock) //actions
                    codeGen = new CodeGenerator(JSON.parse(cmd))
                    try {
                        code = codeGen.codeGen(JSON.parse(actions))
                        await sendMsg(sock, code)
                    }
                    catch (e) {
                        await sendMsg(sock, e)
                    }
                    break
                case 'replay':
                    cmd = await receiveMsg(sock)
                    actions = JSON.parse(await receiveMsg(sock))
                    codeGen = new CodeGenerator(JSON.parse(cmd))
                    codeGen.initForActions(actions)
                    cmd = await receiveMsg(sock)
                    while(cmd !== 'finished')
                    {
                        const code = codeGen.codeGenByIdx(actions, parseInt(cmd))
                        try {
                            await eval(code)
                        }
                        catch(ex) {
                            await sendMsg(sock, ex)
                        }
                        await sendMsg(sock, 'evaluated')
                        cmd = await receiveMsg(sock) //idx
                    }
                    break
                case 'browserConnectionStatus':
                    await sendMsg(sock, recorder.isBrowserConnected().toString())
                    break

            }
            resolve('OK')
        }
        catch(e) {
            reject(e)
            throw e
        }
    })
}


async function loop() {
    let cmd = await receiveMsg(sock)
    processCmd(cmd)
        .catch(e => sock.send(JSON.stringify({error: e})))
        .then(loop)
}

await loop()