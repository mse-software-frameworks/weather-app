# Backend

At the moment our backend is just a simple .NET6 console app, that consumes the aggregated data from the corresponding topic and persists it to the database.

## AOT vs JIT

In which cases would we choose an AOT approach and in which cases would we choose a JIT approach?

### AOT (Ahead of time)

The benefit of AOT compilation is a faster startup time (no additional compilation has to performed during startup) and reduced memory consumption.
The main benefactor are applications running in the cloud. Most often they run on one specific platform and don't need the cross-platform functionality JIT compilation provides.
Quicker start-up times also save costs and increases user experience, because services are dynamically started and shut-down depending on how many resources are currently required.

### JIT (Just in time)

JIT compilation makes it possible to run code on different platforms and still optimize it to some degrees. Advanced features like dynamic code generation are not possible with AOT.

Since JIT build times are often much shorter, combined with easier cross-compatibility and often found hot-reload functionality, the use of JIT builds rather than AOT is often preferred for development. Also, for  applications where long warm-up times are not a disadvantage but require high peak throughput, JIT may be the better option as speculative optimisations are possible through the runtime. 