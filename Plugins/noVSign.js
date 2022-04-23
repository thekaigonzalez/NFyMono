function onNMonoEngineStart(vars) {
    NJVSignUrl(false); // use the false overload to stop usage of vsign,
    // plugins are called before the VSign system, so the implementation allows stopping VSign before it is requested.
}