import React, { Component } from 'react';

export class Users extends Component {
  static displayName = Users.name;

  constructor(props) {
    super(props);
    this.state = { currentCount: 0 };
    this.state = { users: [], loading: true };
  }

  componentDidMount() {
    this.populateUserData();
  }

  static renderUsersTable(users) {
    return (
      <table className='table table-striped' aria-labelledby="tabelLabel">
        <thead>
          <tr>
            <th>Login</th>
          </tr>
        </thead>
        <tbody>
          {users.map(user =>
            <tr key={user}>
              <td>{user}</td>
            </tr>
          )}
        </tbody>
      </table>
    );
  }

  render() {
    let contents = this.state.loading
      ? <p><em>Loading...</em></p>
      : Users.renderUsersTable(this.state.users);

    return (
      <div>
        <h1 id="tabelLabel" >Active user list</h1>
        {contents}
      </div>
    );
  }

  async populateUserData() {
    const response = await fetch('users');
    const data = await response.json();
    console.log(data);
    this.setState({ users: data, loading: false });
  }
}



 
