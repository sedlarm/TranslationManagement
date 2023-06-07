import React, { Component } from 'react';

export class TranslationJobs extends Component {
  static displayName = TranslationJobs.name;

  constructor(props) {
    super(props);
    this.state = { jobs: [], loading: true };
  }

  componentDidMount() {
    this.populateJobsData();
  }

  static renderJobsTable(jobs) {
    return (
      <table className='table table-striped' aria-labelledby="tabelLabel">
        <thead>
          <tr>
            <th>Translator</th>
            <th>Customer name</th>
            <th>Status</th>
            <th>Price</th>
          </tr>
        </thead>
        <tbody>
          {jobs.map(job =>
            <tr key={job.id}>
              <td>{job.translator.name}</td>
              <td>{job.customerName}</td>
              <td>{job.status}</td>
              <td>{job.price}</td>
            </tr>
          )}
        </tbody>
      </table>
    );
  }

  render() {
    let contents = this.state.loading
      ? <p><em>Loading...</em></p>
        : TranslationJobs.renderJobsTable(this.state.jobs);

    return (
      <div>
        <h1 id="tabelLabel" >Translation jobs</h1>
        <p>Recent created jobs</p>
        {contents}
      </div>
    );
  }

    async populateJobsData() {
        const request = new Request("http://localhost:5000/api/jobs", {
            method: "get",
            mode: 'cors',
            headers: {
                Accept: "application/json"
            }
        });
        const response = await fetch(request);
        console.log(response);
        const data = await response.json();
        console.log(data);
        this.setState({ jobs: data, loading: false });
  }
}
