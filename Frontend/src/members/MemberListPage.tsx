import { keepPreviousData, useQuery } from "@tanstack/react-query";
import type { SubmitEvent } from "react";
import { useSearchParams, Link } from "react-router-dom";
import { getMembers } from "./membersApi";
import { ReadPage, pageSize } from "../RouteUtils";

export function MemberListPage() {
  const [searchParams, setSearchParams] = useSearchParams();
  const page = ReadPage(searchParams.get("page"));
  const search = searchParams.get("search")?.trim() ?? "";

  const memberQuery = useQuery({
    queryKey: ["members", { page, pageSize, search }],
    queryFn: () => getMembers({ page, pageSize, search }),
    placeholderData: keepPreviousData,
  });

  function setPage(nextPage: number) {
    const next = new URLSearchParams(searchParams);

    if (nextPage === 1) {
      next.delete("page");
    } else {
      next.set("page", nextPage.toString());
    }

    setSearchParams(next);
  }

  function handleSearch(event: SubmitEvent<HTMLFormElement>) {
    event.preventDefault();
    const next = new URLSearchParams();
    const formData = new FormData(event.currentTarget);
    const value = formData.get("search")?.toString().trim() ?? "";

    if (value) {
      next.set("search", value);
    }

    setSearchParams(next);
  }

  if (memberQuery.isPending) {
    return <p>Loading members...</p>;
  }

  if (memberQuery.isError) {
    return <p>Could not load the members. Is the API running?</p>;
  }

  const result = memberQuery.data;

  return (
    <main>
      <h1>Members</h1>
      <form key={search} onSubmit={handleSearch}>
        <label>
          Search by name or email
          <input type="search" name="search" defaultValue={search} />
        </label>
        <button type="submit">Search</button>
      </form>
      {result.items.length === 0 ? (
        <p>No members found.</p>
      ) : (
        <ul>
          {result.items.map((member) => (
            <li key={member.id}>
              <Link to={`/members/${member.id}`}>
                <strong>{member.name}</strong> || {member.email}
              </Link>
            </li>
          ))}
        </ul>
      )}
      <p>
        Page {result.page} of {result.totalPages}. {result.totalItems} members
        found.
      </p>
      <button
        type="button"
        onClick={() => setPage(result.page - 1)}
        disabled={result.page <= 1 || memberQuery.isFetching}
      >
        Previous
      </button>{" "}
      <button
        type="button"
        onClick={() => setPage(result.page + 1)}
        disabled={result.page >= result.totalPages || memberQuery.isFetching}
      >
        Next
      </button>
      {memberQuery.isFetching && <p>Updating members...</p>}
    </main>
  );
}
