import React, { Component } from 'react';

export class TranslationJobs extends Component {
    static displayName = TranslationJobs.name;
    static unassigned = false;

  constructor(props) {
      super(props);
      this.state = { jobs: [], loading: true };
      this.unassigned = props.unassigned != undefined ? props.unassigned : false;
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
            <tr key={job.id} class={job.translator ? "" : "red"}>
              <td>{job.translator ? job.translator.name : "unassigned"}</td>
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
        : TranslationJobs.renderJobsTable(this.state.jobs, this.state.unassigned);
    let title = this.unassigned ? "Unassigned jobs" : "Recent created jobs";

    return (  
      <div>
        <h1 id="tabelLabel" >Translation jobs</h1>
            <p>{title}</p>
        {contents}
      </div>
    );
  }

    async populateJobsData() {
        let url = "/api/jobs" + (this.unassigned ? "/unassigned" : "");
        const response = await fetch(url, {
            method: "get",
            mode: 'cors',
            headers: {
                "Accept": "application/json"
            }
        });
        console.log(response);
        const data = await response.json();
        console.log(data);
        this.setState({ jobs: data, loading: false });
  }
}
