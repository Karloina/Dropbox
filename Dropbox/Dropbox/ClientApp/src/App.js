import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { Threads } from './components/Threads';
import { Users } from './components/Users';

import './custom.css'

export default class App extends Component {
  static displayName = App.name;

  render () {
    return (
      <Layout>
        <Route exact path='/' component={Home} />
        <Route path='/users-list' component={Users} />
        <Route path='/threads-list' component={Threads} />
      </Layout>
    );
  }
}
