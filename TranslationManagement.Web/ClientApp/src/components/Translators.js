import React, { Component } from 'react';

export class Translators extends Component {
    static displayName = Translators.name;

  constructor(props) {
    super(props);
    this.state = { translators: [], loading: true };
  }

  componentDidMount() {
    this.populateJobsData();
  }

  static renderTranslatorsTable(translators) {
    return (
      <table className='table table-striped' aria-labelledby="tabelLabel">
        <thead>
          <tr>
            <th>Name</th>
            <th>Status</th>
            <th>Hourly rate</th>
          </tr>
        </thead>
        <tbody>
          {translators.map(tr =>
            <tr key={tr.id}>
              <td>{tr.name}</td>
              <td>{tr.status}</td>
              <td>{tr.hourlyrate}</td>
            </tr>
          )}
        </tbody>
      </table>
    );
  }

  render() {
    let contents = this.state.loading
      ? <p><em>Loading...</em></p>
        : Translators.renderTranslatorsTable(this.state.translators);

    return (
      <div>
        <h1 id="tabelLabel" >Translators</h1>
        <p>Recent created partners</p>
        {contents}
      </div>
    );
  }

    async populateJobsData() {
        const request = new Request("http://localhost:5000/api/translators", {
            method: "get",
            mode: 'cors',
            headers: {
                Accept: "application/json"
            }
        });
        const response = await fetch(request);
        const data = await response.json();
        console.log(data);
        this.setState({ translators: data, loading: false });
  }
}
