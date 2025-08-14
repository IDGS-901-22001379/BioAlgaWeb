import 'zone.js/node';
import { bootstrapApplication } from '@angular/platform-browser';
import { AppComponent } from './app/app';
import { appConfig } from './app/app.config';
import { serverConfig } from './app/app.config.server';

const bootstrap = () => bootstrapApplication(AppComponent, {
  ...appConfig,
  providers: [
    ...(appConfig.providers ?? []),
    ...(serverConfig.providers ?? [])
  ]
});

export default bootstrap;
