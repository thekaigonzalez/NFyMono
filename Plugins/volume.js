function onNMonoEngineStart(env) {
    if (env.volume) {
        let v = parseFloat(env.volume)

        NJSetVol(v);
    }
}