import React, { Component } from 'react';

export class Threads extends Component {
  static displayName = Threads.name;

  constructor(props) {
    super(props);
    this.state = { threads: [], loading: true};
  }

  componentDidMount() {
    this.populateThreadData();
    setInterval(()=> this.populateThreadData(), 1000);
  }

  static renderThreadsTable(threads) {
    return (
      <table className='table table-striped' aria-labelledby="tabelLabel">
        <thead>
          <tr>
            <th>Thread</th>
            <th>Status</th>
          </tr>
        </thead>
        <tbody>
          {threads.map(thread =>
            <tr key={thread.guid}>
              <td>{thread.guid}</td>
              <td>{thread.status}</td>
            </tr>
          )}
        </tbody>
      </table>
    );
  }

  render() {
    let contents = this.state.loading
      ? <p><em>Loading...</em></p>
      : Threads.renderThreadsTable(this.state.threads);

    return (
      <div>
        <h1 id="tabelLabel" >Thread list</h1>
        {contents}
      </div>
    );
  }

  async populateThreadData() {
    const response = await fetch('dropbox');
    const data = await response.json();
    this.setState({ threads: data, loading: false });
  }
}
