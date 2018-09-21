#!/usr/bin/env node
'use strict';

const cp = require('child_process');

const execOptions = {
    cwd: process.env.HOME,
    env: Object.assign({}, process.env, {
        DISPLAY: ':0',
        LANG: 'ru_RU.UTF-8'
    }),
    stdio: 'ignore'
};
const terminals = [];
let buffer = '';

process.stdin.on('data', data => {
    buffer += data.toString();
    const commands = buffer.split('\n');
    buffer = commands.pop();
    commands.forEach(runCommand);
});

function runCommand(rawCmd) {
    const cmd = rawCmd.trim();
    switch(cmd) {
    case 'ru':
    case 'us':
        setxkbmap(cmd);
        break;
    case 'term':
        startTerminal();
        break;
    case 'exit':
        terminals.forEach(p => {
            p.kill('SIGHUP');
        });
        process.exit(0);
        break;
    }
}

function setxkbmap(lang) {
    try {
        cp.execSync(`setxkbmap ${lang}`, execOptions);
        sendMessage('ok');
    } catch(err) {
        sendError(err);
    }
}

function startTerminal() {
    const p = cp.spawn('xfce4-terminal', [], execOptions);
    terminals.push(p);
    p.on('exit', () => {
        const index = terminals.indexOf(p);
        if(index < 0) { return; }
        terminals.splice(index, 1);
        if(terminals.length === 0) {
            sendMessage('exit');
        }
    });
}

function sendError(err) {
    process.stderr.write(String(err).replace(/\n/g, '\r').replace(/\r\r/g, '\r') + '\n');
}

function sendMessage(msg) {
    process.stdout.write(`${msg}\n`);
}
