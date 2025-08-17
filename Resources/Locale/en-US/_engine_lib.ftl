# Used internally by the THE() function.
zzzz-the = { PROPER($ent) ->
    *[false] the { $ent }
     [true] { $ent }
    }

# Used internally by the SUBJECT() function.
he-subject = he
she-subject = she
they-subject = they
it-subject = it

# Used internally by the OBJECT() function.
he-object = him
she-object = her
they-object = them
it-object = it

# Used internally by the DAT-OBJ() function.
# Not used in en-US. Created to support other languages.
# (e.g., "to him," "for her")
he-dative = him
she-dative = her
they-dative = them
it-dative = it

# Used internally by the GENITIVE() function.
# Not used in en-US. Created to support other languages.
# e.g., "у него" (Russian), "seines Vaters" (German).
he-genitive = his
she-genitive = her
they-genitive = their
it-genitive = its

# Used internally by the POSS-PRONOUN() function.
he-poss-pronoun = his
she-poss-pronoun = hers
they-poss-pronoun = theirs
it-poss-pronoun = its

# Used internally by the POSS-ADJ() function.
he-poss-adjective = his
she-poss-adjective = her
they-poss-adjective = their
it-poss-adjective = its

# Used internally by the REFLEXIVE() function.
he-reflexive = himself
she-reflexive = herself
they-reflexive = themselves
it-reflexive = itself

# Used internally by the CONJUGATE-BE() function.
be-plural = are
be-singular = is

# Used internally by the CONJUGATE-HAVE() function.
have-plural = have
have-singular = has

# Used internally by the CONJUGATE-BASIC() function.
verb-plural { $first }
verb-singular { $second }
